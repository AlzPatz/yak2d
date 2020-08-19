#include <metal_stdlib>
using namespace metal;

#define ARRAY_SIZE 8

struct LinearSampledGaussianUniforms
{
    float2 TexelShiftSize;
    int NumberOfSamples;
    float Pad0;
    float4 Pad1;
};

struct Factor
{
    float Offset;
    float Weight;
    float2 Pad;
};

struct WeightsAndOffsets
{
    Factor Factors[ARRAY_SIZE];
};

struct FragmentIn
{
    float4 Position [[attribute(0)]];
    float2 FTex [[attribute(1)]];
};

fragment float4 shader( FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> Texture_Source [[ texture(0)]],
                    sampler Sampler_Source [[ sampler(0)]],
                    constant LinearSampledGaussianUniforms &sampleUniforms [[buffer(0)]],
                    constant WeightsAndOffsets &weightsAndOffsets [[buffer(1)]])
                    {
                       	float4 colour = Texture_Source.sample(Sampler_Source, fIn.FTex) * weightsAndOffsets.Factors[0].Weight;

                        for (int n = 1; n < sampleUniforms.NumberOfSamples; n++)
                        {
                            colour += Texture_Source.sample(Sampler_Source, fIn.FTex + (weightsAndOffsets.Factors[n].Offset * sampleUniforms.TexelShiftSize)) * weightsAndOffsets.Factors[n].Weight;
                            colour += Texture_Source.sample(Sampler_Source, fIn.FTex - (weightsAndOffsets.Factors[n].Offset * sampleUniforms.TexelShiftSize)) * weightsAndOffsets.Factors[n].Weight;
                        }

	                    return colour;
                    }