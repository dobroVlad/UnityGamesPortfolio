using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/RP/CustomRPAsset")]
public class CustomRPAsset : RenderPipelineAsset
{
    [SerializeField]
    bool _useDynamicBatching = true;
    [SerializeField]
    bool _useGPUInstancing = true;
    [SerializeField] 
    bool _useSRPBatcher = true;
    [SerializeField]
    ShadowSettings _shadows = default;

    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRP(_useDynamicBatching, _useGPUInstancing, _useSRPBatcher, _shadows);
    }
}