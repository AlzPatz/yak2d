cbuffer MixFactors: register(b0)
{
	float4 Amount;
};

SamplerState SamplerMix;
Texture2D TextureMix;

SamplerState Sampler0;
Texture2D Texture0;

SamplerState Sampler1;
Texture2D Texture1;

SamplerState Sampler2;
Texture2D Texture2;

SamplerState Sampler3;
Texture2D Texture3;

struct FragmentIn
{
	float4 Position : SV_Position;
	float2 FTex : TEXCOORD;
};

float4 main(FragmentIn fIn) : SV_Target
{
	float4 mixing = TextureMix.Sample(SamplerMix, fIn.FTex);
	
	return   		(Amount.r * mixing.r * Texture0.Sample(Sampler0, fIn.FTex)) + 
				(Amount.g * mixing.g * Texture1.Sample(Sampler1, fIn.FTex)) + 
    				(Amount.b * mixing.b * Texture2.Sample(Sampler2, fIn.FTex)) + 
    				(Amount.a * mixing.a * Texture3.Sample(Sampler3, fIn.FTex));
}
