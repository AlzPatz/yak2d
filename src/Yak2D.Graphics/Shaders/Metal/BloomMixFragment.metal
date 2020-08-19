#include <metal_stdlib>
using namespace metal;

struct UniformBlock
{
    float MixAmount;
	float Pad0;
	float Pad1;
	float Pad2;
	float4 Pad3;
};

struct FragmentIn
{
    float4 Position [[attribute(0)]];
    float2 FTex [[attribute(1)]];
};

fragment float4 shader( FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> Texture_Source [[ texture(0)]],
                    sampler Sampler_Source [[ sampler(0)]],
                    texture2d<float, access::sample> Texture_Processed [[ texture(1)]],
                    sampler Sampler_Processed [[ sampler(1)]],
                    constant UniformBlock &uniformBlock [[buffer(0)]])
                    {
                       	float4 source = Texture_Source.sample(Sampler_Source, fIn.FTex);
	                    float4 processed = Texture_Processed.sample(Sampler_Processed, fIn.FTex);

	                    return source + (uniformBlock.MixAmount * processed);
                    }