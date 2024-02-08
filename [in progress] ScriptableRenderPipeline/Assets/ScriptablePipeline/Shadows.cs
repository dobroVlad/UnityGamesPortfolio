using UnityEngine;
using UnityEngine.Rendering;
public class Shadows
{
    struct ShadowedDirectionalLight
    {
        public int visibleLightIndex;
        public float slopeScaleBias;
        public float nearPlaneOffset;
    }
    const string bufferName = "Shadows";
    const int maxShadowedDirectionalLightCount = 4;
    const int maxCascades = 4;
    static int dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");
    static int dirShadowMatricesId = Shader.PropertyToID("_DirectionalShadowMatrices");
    static int cascadeCountId = Shader.PropertyToID("_CascadeCount");
    static int cascadeCullingSpheresId = Shader.PropertyToID("_CascadeCullingSpheres");
    static int cascadeDataId = Shader.PropertyToID("_CascadeData");
    static int shadowAtlasSizeId = Shader.PropertyToID("_ShadowAtlasSize");
    static int shadowDistanceFadeId = Shader.PropertyToID("_ShadowDistanceFade");
    static string[] directionalFilterKeywords = {
        "_DIRECTIONAL_PCF3",
        "_DIRECTIONAL_PCF5",
        "_DIRECTIONAL_PCF7",
    };
    static string[] cascadeBlendKeywords = {
        "_CASCADE_BLEND_SOFT",
        "_CASCADE_BLEND_DITHER"
    };
    static string[] shadowMaskKeywords = {
       "_SHADOW_MASK_ALWAYS",
        "_SHADOW_MASK_DISTANCE"
    };
    bool useShadowMask;
    static Matrix4x4[] dirShadowMatrices = new Matrix4x4[maxShadowedDirectionalLightCount * maxCascades];
    static Vector4[] cascadeCullingSpheres = new Vector4[maxCascades];
    static Vector4[] cascadeData = new Vector4[maxCascades];

    int ShadowedDirectionalLightCount;
    CommandBuffer buffer = new CommandBuffer { name = bufferName };
    ScriptableRenderContext context;
    CullingResults cullingResults;
    ShadowSettings settings;
    ShadowedDirectionalLight[] ShadowedDirectionalLights = new ShadowedDirectionalLight[maxShadowedDirectionalLightCount];

    public void Setup( ScriptableRenderContext context, CullingResults cullingResults, ShadowSettings settings)
    {
        this.context = context;
        this.cullingResults = cullingResults;
        this.settings = settings;
        ShadowedDirectionalLightCount = 0;
        useShadowMask = false;
    }
    public Vector4 ReserveDirectionalShadows(Light light, int visibleLightIndex)
    {
        if (ShadowedDirectionalLightCount < maxShadowedDirectionalLightCount &&
            light.shadows != LightShadows.None &&
            light.shadowStrength > 0f)
        {
            LightBakingOutput lightBaking = light.bakingOutput;
            float maskChannel = -1;
            if (
                lightBaking.lightmapBakeType == LightmapBakeType.Mixed &&
                lightBaking.mixedLightingMode == MixedLightingMode.Shadowmask
            )
            {
                useShadowMask = true;
                maskChannel = lightBaking.occlusionMaskChannel;
            }
            if (!cullingResults.GetShadowCasterBounds( visibleLightIndex, out Bounds b))
            {
                return new Vector4(-light.shadowStrength, 0f, 0f, maskChannel);
            }
            ShadowedDirectionalLights[ShadowedDirectionalLightCount] =
                new ShadowedDirectionalLight
                {
                    visibleLightIndex = visibleLightIndex,
                    slopeScaleBias = light.shadowBias,
                    nearPlaneOffset = light.shadowNearPlane
                };
            return new Vector4(
                light.shadowStrength,
                settings.directional.cascadeCount * ShadowedDirectionalLightCount++,
                light.shadowNormalBias,
                maskChannel
            );
        }
        return new Vector4(0f, 0f, 0f, -1f);
    }
    public void Render()
    {
        if (ShadowedDirectionalLightCount > 0)
        {
            RenderDirectionalShadows();
        }
        else
        {
            buffer.GetTemporaryRT(
                dirShadowAtlasId,
                1,
                1,
                32,
                FilterMode.Bilinear,
                RenderTextureFormat.Shadowmap
            );
        }
        buffer.BeginSample(bufferName);
        SetKeywords(shadowMaskKeywords, useShadowMask ? QualitySettings.shadowmaskMode == ShadowmaskMode.Shadowmask ? 0 : 1 : -1);
        buffer.EndSample(bufferName);
        ExecuteBuffer();
    }
    void RenderDirectionalShadows()
    {
        int atlasSize = (int)settings.directional.atlasSize;
        buffer.GetTemporaryRT(
            dirShadowAtlasId,
            atlasSize,
            atlasSize,
            32,
            FilterMode.Bilinear,
            RenderTextureFormat.Shadowmap);
        buffer.SetRenderTarget( dirShadowAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        buffer.ClearRenderTarget(true, false, Color.clear);
        buffer.BeginSample(bufferName);
        ExecuteBuffer();
        int tiles = ShadowedDirectionalLightCount * settings.directional.cascadeCount;
        int split = tiles <= 1 ? 1 : tiles <= 4 ? 2 : 4;
        int tileSize = atlasSize / split;
        for (int i = 0; i < ShadowedDirectionalLightCount; i++)
        {
            RenderDirectionalShadows(i, split, tileSize);
        }
        buffer.SetGlobalInt(cascadeCountId, settings.directional.cascadeCount);
        buffer.SetGlobalVectorArray( cascadeCullingSpheresId, cascadeCullingSpheres );
        buffer.SetGlobalVectorArray(cascadeDataId, cascadeData);
        buffer.SetGlobalMatrixArray(dirShadowMatricesId, dirShadowMatrices);
        float f = 1f - settings.directional.cascadeFade;
        buffer.SetGlobalVector(
            shadowDistanceFadeId, 
            new Vector4( 1f / settings.maxDistance, 1f / settings.distanceFade, 1f / (1f - f * f) ) );
        SetKeywords( directionalFilterKeywords, (int)settings.directional.filter - 1 );
        SetKeywords( cascadeBlendKeywords, (int)settings.directional.cascadeBlend - 1 );
        buffer.SetGlobalVector( shadowAtlasSizeId, new Vector4(atlasSize, 1f / atlasSize));
        buffer.EndSample(bufferName);
        ExecuteBuffer();
    }
    void RenderDirectionalShadows(int index, int split, int tileSize) 
    {
        ShadowedDirectionalLight light = ShadowedDirectionalLights[index];
        var shadowSettings = new ShadowDrawingSettings( cullingResults, light.visibleLightIndex);
        int cascadeCount = settings.directional.cascadeCount;
        int tileOffset = index * cascadeCount;
        Vector3 ratios = settings.directional.CascadeRatios;
        float cullingFactor = Mathf.Max(0f, 0.8f - settings.directional.cascadeFade);
        for (int i = 0; i < cascadeCount; i++)
        {
            cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(
                  light.visibleLightIndex,
                  i,
                  cascadeCount,
                  ratios,
                  tileSize,
                  light.nearPlaneOffset, 
                  out Matrix4x4 viewMatrix,
                  out Matrix4x4 projectionMatrix,
                  out ShadowSplitData splitData
              );
            splitData.shadowCascadeBlendCullingFactor = cullingFactor;
            shadowSettings.splitData = splitData;
            if (index == 0)
            {
                SetCascadeData(i, splitData.cullingSphere, tileSize);
            }
            int tileIndex = tileOffset + i;
            dirShadowMatrices[tileIndex] = ConvertToAtlasMatrix(
                projectionMatrix * viewMatrix,
                SetTileViewport(tileIndex, split, tileSize), split
            );
            buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
            buffer.SetGlobalDepthBias(0f, light.slopeScaleBias);
            ExecuteBuffer();
            context.DrawShadows(ref shadowSettings);
            buffer.SetGlobalDepthBias(0f, 0f);
        }
    }
    void SetCascadeData(int index, Vector4 cullingSphere, float tileSize)
    {
        float texelSize = 2f * cullingSphere.w / tileSize;
        float filterSize = texelSize * ((float)settings.directional.filter + 1f);
        cullingSphere.w -= filterSize;
        cullingSphere.w *= cullingSphere.w;
        cascadeCullingSpheres[index] = cullingSphere;
        cascadeData[index] = new Vector4(
            1f / cullingSphere.w,
           filterSize * 1.4142136f);
    }
    Vector2 SetTileViewport(int index, int split, float tileSize)
    {
        Vector2 offset = new Vector2(index % split, index / split);
        buffer.SetViewport(new Rect(
            offset.x * tileSize, offset.y * tileSize, tileSize, tileSize
        ));
        return offset;
    }
    Matrix4x4 ConvertToAtlasMatrix(Matrix4x4 m, Vector2 offset, int split)
    {
        if (SystemInfo.usesReversedZBuffer)
        {
            m.m20 = -m.m20;
            m.m21 = -m.m21;
            m.m22 = -m.m22;
            m.m23 = -m.m23;
        }
        float scale = 1f / split;
        m.m00 *= 0.5f * scale;
        m.m01 *= 0.5f * scale;
        m.m02 *= 0.5f * scale;
        m.m03 = (0.5f * (m.m03 + 1f) + offset.x) * scale;
        m.m10 *= 0.5f * scale;
        m.m11 *= 0.5f * scale;
        m.m12 *= 0.5f * scale;
        m.m13 = (0.5f * (m.m13 + 1f) + offset.y) * scale;
        m.m20 *= 0.5f;
        m.m21 *= 0.5f;
        m.m22 *= 0.5f;
        m.m23 = 0.5f * (m.m23 + 1f);
        return m;
    }
    void SetKeywords(string[] keywords, int enabledIndex)
    {
        for (int i = 0; i < keywords.Length; i++)
        {
            if (i == enabledIndex)
            {
                buffer.EnableShaderKeyword(keywords[i]);
            }
            else
            {
                buffer.DisableShaderKeyword(keywords[i]);
            }
        }
    }
    public void Cleanup()
    {
        buffer.ReleaseTemporaryRT(dirShadowAtlasId);
        ExecuteBuffer();
    }
    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}