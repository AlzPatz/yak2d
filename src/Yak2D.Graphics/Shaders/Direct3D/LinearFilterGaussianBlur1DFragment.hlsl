#define ARRAY_SIZE 8

//Requires linear sampling of fIn texture
//http://rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/

SamplerState Sampler_Source : register(s0);
Texture2D Texture_Source : register(t0);

cbuffer LinearSampledGaussianUniforms : register(b0)
{
	float2 TexelShiftSize;
	int NumberOfSamples;
	float Pad0;
	float4 Pad1;
};

struct Factor
{
	float Offset;
	float Weight;
	float2 Pad;
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
	float4 colour = Texture_Source.Sample(Sampler_Source, fIn.FTex) * Factors[0].Weight;

	for (int n = 1; n < NumberOfSamples; n++)
	{
		colour += Texture_Source.Sample(Sampler_Source, fIn.FTex + (Factors[n].Offset * TexelShiftSize)) * Factors[n].Weight;
		colour += Texture_Source.Sample(Sampler_Source, fIn.FTex - (Factors[n].Offset * TexelShiftSize)) * Factors[n].Weight;
	}

	return colour;
}