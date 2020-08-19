#include <metal_stdlib>
using namespace metal;

struct VertexIn
{
    float3 Position [[attribute(0)]];
	float2 VTex [[attribute(1)]];
};

struct FragmentIn
{
	float4 Position [[position]];
	float2 FTex; 
};

vertex FragmentIn shader(VertexIn vIn [[stage_in]])
{
    FragmentIn output;
	output.Position = float4(vIn.Position, 1.0);
	output.FTex = vIn.VTex;
	return output;
}