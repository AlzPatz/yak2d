#include <metal_stdlib>
using namespace metal;

struct FragmentIn
{
    float4 Position [[attribute(0)]];
    uint FTexturingType [[attribute(1)]]; //[[flat]] ?
    float4 FColor [[attribute(2)]];
    float2 FTexCoord0 [[attribute(3)]];
    float2 FTexCoord1 [[attribute(4)]];
    float FTex0Weight [[attribute(5)]];
};

fragment float4 shader( FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> tex0[[ texture(0)]],
                    sampler texSampler0 [[ sampler(0)]],
                    texture2d<float, access::sample> tex1 [[ texture(1)]],
                    sampler texSampler1 [[ sampler(1)]])
                    {
                        if (fIn.FTexturingType == 0)
                        {
                            //Colour Only
                            return fIn.FColor;
                        }

                        if (fIn.FTexturingType == 1)
                        {
                            //Single Texture
                            return fIn.FColor * tex0.sample(texSampler0, fIn.FTexCoord0);
                        }

                        //Dual Texture
                        return fIn.FColor *
                        (
                            (tex0.sample(texSampler0, fIn.FTexCoord0) * fIn.FTex0Weight) +
                            (tex1.sample(texSampler1, fIn.FTexCoord1) * (1.0 - fIn.FTex0Weight))
                        );
                    }