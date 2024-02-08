using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CustomCameraRenderer
{
    private ScriptableRenderContext _context;
    private Camera _camera;
    private const string CommandBufferLabel = "Render Target";
    private CommandBuffer _buffer = new CommandBuffer
    {
        name = CommandBufferLabel
    };
    private static readonly ShaderTagId _unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    private static readonly ShaderTagId _litShaderTagId = new ShaderTagId("CustomLit");
    private CullingResults _cullingResults;
    Lighting lighting = new Lighting();
    public void Render(
        ScriptableRenderContext context, 
        Camera camera,
        bool useDynamicBatching,
        bool useGPUInstancing,
        ShadowSettings shadowSettings)
    {
        _context = context;
        _camera = camera;
        PrepareBuffer();
        PrepareForSceneWindow();
        if (!TryCull(shadowSettings.maxDistance))
        { return; }
        _buffer.BeginSample(SampleName);
        ExecuteBuffer();
        lighting.Setup(context, _cullingResults, shadowSettings);
        _buffer.EndSample(SampleName);
        CameraRenderingSetup();
        DrawUnsupportedShaders();
        DrawGeometryData(useDynamicBatching, useGPUInstancing);
        DrawGizmos();
        lighting.Cleanup();
        SubmitRenderingData();
    }
    private void CameraRenderingSetup()
    {
        _context.SetupCameraProperties(_camera);
        CameraClearFlags flags = _camera.clearFlags;
        _buffer.ClearRenderTarget(
            flags <= CameraClearFlags.Depth, 
            flags == CameraClearFlags.Color,
            flags == CameraClearFlags.Color ?
                _camera.backgroundColor.linear : Color.clear);
        _buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }
    private void DrawGeometryData(bool useDynamicBatching, bool useGPUInstancing)
    {
        var sortingSettings = new SortingSettings(_camera);
        sortingSettings.criteria = SortingCriteria.CommonOpaque;
        var drawingSettings = new DrawingSettings(_unlitShaderTagId, sortingSettings)
        {
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing,
            perObjectData =
                PerObjectData.ReflectionProbes |
                PerObjectData.Lightmaps | 
                PerObjectData.ShadowMask |
                PerObjectData.LightProbe |
                PerObjectData.OcclusionProbe |
                PerObjectData.LightProbeProxyVolume |
                PerObjectData.OcclusionProbeProxyVolume
        };
        drawingSettings.SetShaderPassName(1, _litShaderTagId);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
        _context.DrawSkybox(_camera);
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
    }
    partial void DrawUnsupportedShaders();
    partial void DrawGizmos();
    partial void PrepareForSceneWindow();
    partial void PrepareBuffer();
    private void SubmitRenderingData()
    {
        _buffer.EndSample(SampleName);
        ExecuteBuffer();
        _context.Submit();
    }
    private void ExecuteBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
    bool TryCull(float maxShadowDistance)
    {
        if (_camera.TryGetCullingParameters(out ScriptableCullingParameters cullingParameters))
        {
            cullingParameters.shadowDistance = Mathf.Min(maxShadowDistance, _camera.farClipPlane); ;
            _cullingResults = _context.Cull(ref cullingParameters);
            return true;
        }
        return false;
    }
}
