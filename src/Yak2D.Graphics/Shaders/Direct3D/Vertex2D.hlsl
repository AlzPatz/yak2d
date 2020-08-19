struct VertexIn
{
	float3 Position : POSITION0;
	float2 VTex : TEXCOORD;
};

struct FragmentIn
{
	float4 Position : SV_Position;
	float2 FTex : TEXCOORD;
};

FragmentIn main(VertexIn vIn)
{
	FragmentIn output;
	output.Position = float4(vIn.Position, 1.0);
	output.FTex = vIn.VTex;
	return output;
}