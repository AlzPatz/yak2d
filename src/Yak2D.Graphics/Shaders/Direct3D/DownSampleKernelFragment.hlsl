#define ARRAY_SIZE 16

SamplerState Sampler : register(s0);
Texture2D Texture : register(t0);

cbuffer SampleUniforms : register(b0)
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

cbuffer WeightsAndOffsets : register (b1)
{
	Factor Factors[ARRAY_SIZE];
};

struct FragmentIn
{
	float4 Position : SV_Position;
	float2 FTex : TEXCOORD;
};

float4 main(FragmentIn input) : SV_Target
{
	float4 sum = float4(0,0,0,0);

	for (int i = 0; i < NumberOfSamples; i++)
	{
		sum += Factors[i].Weight * Texture.Sample(Sampler, input.FTex + (Factors[i].Offset * TexelSize));
	}

	return sum;
}