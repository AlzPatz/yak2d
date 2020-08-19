SamplerState texSampler0 : register(s0);
Texture2D tex0 : register(t0);

SamplerState texSampler1 : register(s1);
Texture2D tex1 : register(t1);

struct FragmentIn
{
	float4 Position : SV_Position;
	nointerpolation int FTexturingType : TYPE;
	float4 FColor : COLOR2;
	float2 FTexCoord0 : TEXCOORD0;
	float2 FTexCoord1 : TEXCOORD1;
	float FTex0Weight : WEIGHT;
};

float4 main(FragmentIn fIn) : SV_Target
{
	if (fIn.FTexturingType == 0)
	{
		//Colour Only
		return fIn.FColor;
	}

	if (fIn.FTexturingType == 1)
	{
		//Single Texture
		return fIn.FColor * tex0.Sample(texSampler0, fIn.FTexCoord0);
	}

	//Dual Texture
	return fIn.FColor *
	(
		(tex0.Sample(texSampler0, fIn.FTexCoord0) * fIn.FTex0Weight) +
		(tex1.Sample(texSampler1, fIn.FTexCoord1) * (1.0 - fIn.FTex0Weight))
	);
}