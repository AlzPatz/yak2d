SamplerState Sampler : register(s0);
Texture2D Texture : register(t0);

struct FragmentIn
{
	float4 Position : SV_Position;
	float2 FTex : TEXCOORD;
};

float4 main(FragmentIn fIn) : SV_Target
{
	return Texture.Sample(Sampler, fIn.FTex);
}