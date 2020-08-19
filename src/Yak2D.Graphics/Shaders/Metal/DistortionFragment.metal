//All forms of distortion fragment do not use the uniform PixelSizeFactor... should it be renamed PixelSize not PixelShift and 
//should it scale some tex coord mods below?

#include <metal_stdlib>
using namespace metal;

struct DistortionFactor
{
    float2 DistortionScalar;
    float2 Pad0;
    float4 Pad1;	
};

struct FragmentIn
{
    float4 Position [[attribute(0)]];
    float2 FTex [[attribute(1)]];
};

fragment float4 shader( FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> TextureGradientMap [[ texture(0)]],
                    sampler SamplerGradientMap [[ sampler(0)]],
                    texture2d<float, access::sample> TextureImage [[ texture(1)]],
                    sampler SamplerImage [[ sampler(1)]],                    
                    constant DistortionFactor &distortionFactor [[buffer(0)]])
                    {
                        float x = TextureGradientMap.sample(SamplerGradientMap, fIn.FTex).r;
                        float y = TextureGradientMap.sample(SamplerGradientMap, fIn.FTex).g;
                        float2 pixelShift = distortionFactor.DistortionScalar * float2(x, y);
                        return TextureImage.sample(SamplerImage, fIn.FTex + pixelShift);		
                    }