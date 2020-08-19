#include <metal_stdlib>
using namespace metal;

#define ARRAY_SIZE 16

struct SampleUniforms
{
    float2 TexelSize;
    int NumberOfSamples;
    float Pad0;
    float4 Pad1;
};

struct Factor
{
    float2 Offset;
    float Weight;
    float Pad;
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

fragment float4 shader(FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> Texture [[ texture(0)]],
                    sampler Sampler [[ sampler(0)]],
                    constant SampleUniforms &sampleUniforms [[buffer(0)]],
                    constant WeightsAndOffsets &weightsAndOffsets [[buffer(1)]])
                    {
                        float4 sum = float4(0,0,0,0);

                        for (int i = 0; i < sampleUniforms.NumberOfSamples; i++)
                        {
                            sum += weightsAndOffsets.Factors[i].Weight * 
                                            Texture.sample(Sampler, fIn.FTex + 
                                            (weightsAndOffsets.Factors[i].Offset * sampleUniforms.TexelSize));
                        }
                        return sum;    
                    }