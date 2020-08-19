//Simple Vertex Shader to feed Phong Lighting Model Fragment Shader

//The supplied WVP matrix assumes a right handed coordinate system

cbuffer VertexUniforms : register(b0)
{
	float4x4 WorldViewProjection;
};

struct VertexIn
{
	float3 VertPosition : POSITION;
	float3 VertNormal : NORMAL;
	float2 VertTexCoord : TEXCOORD;
};

struct FragmentIn
{
	float4 Position : SV_Position;
	float3 FragPosition : POSITION;
	float3 FragNormal: NORMAL;
	float2 FragTexCoord : TEXCOORD;
};

FragmentIn main(VertexIn vIn)
{	
	FragmentIn vOut;
	
	float4 pos = float4(vIn.VertPosition.x, vIn.VertPosition.y, vIn.VertPosition.z, 1.0);

	vOut.Position = mul(WorldViewProjection, pos); 

	vOut.FragTexCoord = vIn.VertTexCoord;
	vOut.FragNormal = vIn.VertNormal; 
	vOut.FragPosition = vIn.VertPosition;

	return vOut;
}