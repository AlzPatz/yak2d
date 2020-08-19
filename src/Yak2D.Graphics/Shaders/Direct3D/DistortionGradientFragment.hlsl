cbuffer PixelSizeFactor : register(b0)
{
	float2 PixelShift;
    float2 Pad0;
    float4 Pad1;
};

SamplerState SamplerHeightMap : register(s0);
Texture2D TextureHeightMap : register(t0);

struct FragmentIn
{
	float4 Position : SV_Position;
	float2 FTex : TEXCOORD;
};

float2 main(FragmentIn fIn) : SV_Target
{
    float right = TextureHeightMap.Sample(SamplerHeightMap, fIn.FTex + float2(PixelShift.x, 0.0)).r;
    float left = TextureHeightMap.Sample(SamplerHeightMap, fIn.FTex - float2(PixelShift.x, 0.0)).r;
    
    float top = TextureHeightMap.Sample(SamplerHeightMap, fIn.FTex + float2(0.0, PixelShift.y)).r;
    float bottom = TextureHeightMap.Sample(SamplerHeightMap, fIn.FTex - float2(0.0f, PixelShift.y)).r;

    float centre = TextureHeightMap.Sample(SamplerHeightMap, fIn.FTex).r;

    float horizontal = 0.5 * ((centre - left) + (right - centre));
    float vertical = 0.5 * ((centre - bottom) + (top - centre)); 
    
    return float2(horizontal, vertical);	
}