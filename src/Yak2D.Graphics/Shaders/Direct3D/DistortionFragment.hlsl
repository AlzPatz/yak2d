cbuffer DistortionFactor : register(b0)
{
	float2 DistortionScalar;
    float2 Pad2;
    float4 Pad3;
}

SamplerState SamplerGradientMap : register(s0);
Texture2D TextureGradientMap : register(t0);

SamplerState SamplerImage : register(s1);
Texture2D TextureImage : register(t1);

struct FragmentIn
{
	float4 Position : SV_Position;
	float2 FTex : TEXCOORD;
};

float4 main(FragmentIn fIn) : SV_Target
{
    float x = TextureGradientMap.Sample(SamplerGradientMap, fIn.FTex).r;
    float y = TextureGradientMap.Sample(SamplerGradientMap, fIn.FTex).g;
    float2 pixelShift = DistortionScalar * float2(x, y);
    return TextureImage.Sample(SamplerImage, fIn.FTex + pixelShift);		
}