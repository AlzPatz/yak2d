//Simple Vertex Shader to feed Phong Lighting Model Fragment Shader

//The supplied WVP matrix assumes a right handed coordinate system

#include <metal_stdlib>
using namespace metal;

struct VertexUniforms
{
    float4x4 WorldViewProjection;
};

struct VertexIn
{
    float3 VertPosition [[attribute(0)]];
    float3 VertNormal [[attribute(1)]];
	float2 VertTexCoord [[attribute(2)]];
};

struct FragmentIn
{
	float4 Position [[position]];
    float3 FragPosition;
    float3 FragNormal;
	float2 FragTexCoord; 
};

vertex FragmentIn shader(VertexIn vIn [[stage_in]],
                    constant VertexUniforms &vertexUniforms [[buffer(0)]])
{
    FragmentIn vOut;
	
	float4 pos = float4(vIn.VertPosition.x, vIn.VertPosition.y, vIn.VertPosition.z, 1.0);

	vOut.Position = vertexUniforms.WorldViewProjection * pos; 

	vOut.FragTexCoord = vIn.VertTexCoord;
	vOut.FragNormal = vIn.VertNormal; 
	vOut.FragPosition = vIn.VertPosition;

	return vOut;
}