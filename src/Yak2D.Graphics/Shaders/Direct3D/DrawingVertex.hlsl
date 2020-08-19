cbuffer WorldViewProjection : register(b0)
{
	float4x4 WorldMatrix;
	float4x4 ScreenMatrix;
};

struct VertexIn
{
	int isWorld : TEXCOORD0;
	int TexturingType : TEXCOORD1;
	float3 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TexCoord0 : TEXCOORD2;
	float2 TexCoord1 : TEXCOORD3;
	float TexWeight0 : TEXCOORD4;
};

struct FragmentIn
{
	float4 Position : SV_Position;
	nointerpolation int FTexturingType : TYPE;
	float4 FColor : COLOR2;
	float2 FTexCoord0 : TEXCOORD0;
	float2 FTexCoord1 : TEXCOORD1;
	float FTex0Weight : WEIGHT;
};

FragmentIn main(VertexIn vIn)
{
	float4 pos = float4(vIn.Position.x, vIn.Position.y, vIn.Position.z, 1.0);

	FragmentIn vOut;

	if (vIn.isWorld == 0)
	{
		vOut.Position = mul(WorldMatrix, pos);
	}
	else
	{
		vOut.Position = mul(ScreenMatrix, pos);
	}

	vOut.Position.z = -vOut.Position.z; 

	vOut.FColor = vIn.Color;
	vOut.FTexturingType = vIn.TexturingType;

	vOut.FTexCoord0 = vIn.TexCoord0;
	vOut.FTexCoord1 = vIn.TexCoord1;
	
	vOut.FTex0Weight = vIn.TexWeight0;

	return vOut;
}