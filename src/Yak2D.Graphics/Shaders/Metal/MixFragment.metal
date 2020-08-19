#include <metal_stdlib>
using namespace metal;

struct MixFactors
{
    float4 Amount;
};

struct FragmentIn
{
    float4 Position [[attribute(0)]];
    float2 FTex [[attribute(1)]];
};

fragment float4 shader( FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> TextureMix[[ texture(0)]],
                    sampler SamplerMix [[ sampler(0)]],
                    texture2d<float, access::sample> Texture0[[ texture(1)]],
                    sampler Sampler0 [[ sampler(1)]],
                    texture2d<float, access::sample> Texture1 [[ texture(2)]],
                    sampler Sampler1 [[ sampler(2)]],
                    texture2d<float, access::sample> Texture2[[ texture(3)]],
                    sampler Sampler2 [[ sampler(3)]],
                    texture2d<float, access::sample> Texture3 [[ texture(4)]],
                    sampler Sampler3 [[ sampler(4)]],
                    constant MixFactors &mixFactors [[buffer(0)]])
                    {
	                    float4 mixing = TextureMix.sample(SamplerMix, fIn.FTex);
	
	                    return  (mixFactors.Amount.r * mixing.r * Texture0.sample(Sampler0, fIn.FTex)) + 
				                (mixFactors.Amount.g * mixing.g * Texture1.sample(Sampler1, fIn.FTex)) + 
    				            (mixFactors.Amount.b * mixing.b * Texture2.sample(Sampler2, fIn.FTex)) + 
    				            (mixFactors.Amount.a * mixing.a * Texture3.sample(Sampler3, fIn.FTex));
                    }