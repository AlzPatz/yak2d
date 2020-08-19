SamplerState Sampler_Source : register(s0);
Texture2D Texture_Source : register(t0);

SamplerState Sampler_Processed : register(s1);
Texture2D Texture_Processed : register(t1);

cbuffer UniformBlock : register(b0)
{
	float MixAmount;
	float Pad0;
	float Pad1;
	float Pad2;
	float4 Pad3;
};

struct FragmentIn
{
	float4 Position : SV_Position;
	float2 FTex : TEXCOORD;
};

float4 main(FragmentIn fIn) : SV_Target
{
	float4 source = Texture_Source.Sample(Sampler_Source, fIn.FTex);
	float4 processed = Texture_Processed.Sample(Sampler_Processed, fIn.FTex);

	return lerp(source, processed, MixAmount);
}