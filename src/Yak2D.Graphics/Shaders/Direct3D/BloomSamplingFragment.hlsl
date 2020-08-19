#define ARRAY_SIZE 16

SamplerState Sampler : register(s0);
Texture2D Texture : register(t0);

cbuffer SampleUniforms : register(b0)
{
	float2 TexelSize;
	int NumberOfSamples;
	float BrightnessThreshold;
	float4 Pad0;
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

float4 main(FragmentIn fIn) : SV_Target
{
	float4 sum = float4(0,0,0,0);

	for (int i = 0; i < NumberOfSamples; i++)
	{
		sum += Factors[i].Weight * Texture.Sample(Sampler, fIn.FTex + (Factors[i].Offset * TexelSize));
	}

	float brightness = dot(sum.rgb, float3(0.2126, 0.7152, 0.0722));
	if (brightness < BrightnessThreshold)
	{
		sum = float4(0,0,0,0);
	}

	return sum;
}