#include <metal_stdlib>
using namespace metal;

struct FragmentIn
{
    float4 Position [[attribute(0)]];
    float2 FTex [[attribute(1)]];
};

fragment float4 shader( FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> Texture [[ texture(0)]],
                    sampler Sampler [[ sampler(0)]])
                    {
                        return Texture.sample(Sampler, fIn.FTex);
                    }