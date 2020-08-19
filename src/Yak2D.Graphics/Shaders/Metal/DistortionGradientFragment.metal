#include <metal_stdlib>
using namespace metal;

struct PixelSizeFactor
{
    float2 PixelShift;	
    float2 Pad0;
    float4 Pad1;
};

struct FragmentIn
{
    float4 Position [[attribute(0)]];
    float2 FTex [[attribute(1)]];
};

fragment float2 shader( FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> TextureHeightMap [[ texture(0)]],
                    sampler SamplerHeightMap [[ sampler(0)]],
                    constant PixelSizeFactor &pixelSizeFactor [[buffer(0)]])
                    {
                        float right = TextureHeightMap.sample(SamplerHeightMap, fIn.FTex + float2(pixelSizeFactor.PixelShift.x, 0.0)).r;
                        float left = TextureHeightMap.sample(SamplerHeightMap, fIn.FTex - float2(pixelSizeFactor.PixelShift.x, 0.0)).r;
    
                        float top = TextureHeightMap.sample(SamplerHeightMap, fIn.FTex + float2(0.0, pixelSizeFactor.PixelShift.y)).r;
                        float bottom = TextureHeightMap.sample(SamplerHeightMap, fIn.FTex - float2(0.0f, pixelSizeFactor.PixelShift.y)).r;

                        float centre = TextureHeightMap.sample(SamplerHeightMap, fIn.FTex).r;

                        float horizontal = 0.5 * ((centre - left) + (right - centre));
                        float vertical = 0.5 * ((centre - bottom) + (top - centre)); 
    
                        return float2(horizontal, vertical);	
                    }