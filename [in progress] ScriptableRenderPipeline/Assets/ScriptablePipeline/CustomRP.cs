using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRP : RenderPipeline
{
    bool _useDynamicBatching;
    bool _useGPUInstancing;
    private CustomCameraRenderer _renderer = new CustomCameraRenderer();
    ShadowSettings _shadowSettings;

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            _renderer.Render(context, camera, _useDynamicBatching, _useGPUInstancing, _shadowSettings);
        }
    }
    public CustomRP(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher, ShadowSettings shadowSettings)
    {
        _useDynamicBatching = useDynamicBatching;
        _useGPUInstancing = useGPUInstancing;
        _shadowSettings = shadowSettings;
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        GraphicsSettings.lightsUseLinearIntensity = true;
    }
}
