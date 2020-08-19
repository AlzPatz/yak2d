#include <metal_stdlib>
using namespace metal;

struct WorldViewProjection
{
	float4x4 WorldMatrix;
    float4x4 ScreenMatrix;
};

struct VertexIn
{
    uint isWorld [[attribute(0)]];
    uint TexturingType [[attribute(1)]];
    float3 Position [[attribute(2)]];
	float4 Color [[attribute(3)]];
	float2 TexCoord0 [[attribute(4)]];
	float2 TexCoord1 [[attribute(5)]];
    float TexWeight0 [[attribute(6)]];
};

struct FragmentIn
{
	float4 Position [[position]];
    uint FTexturingType; [[flat]] //Does it need to be flat also in the fragment shader
	float4 FColor;
	float2 FTexCoord0; 
	float2 FTexCoord1;
    float FTex0Weight; 
};

vertex FragmentIn shader(VertexIn vIn [[stage_in]],
					constant WorldViewProjection &worldViewProjection [[buffer(0)]])
{
	float4 pos = float4(vIn.Position.x, vIn.Position.y, vIn.Position.z, 1.0);

	FragmentIn vOut;

	if (vIn.isWorld == 0)
	{
		vOut.Position = worldViewProjection.WorldMatrix * pos;
	}
	else
	{
		vOut.Position = worldViewProjection.ScreenMatrix * pos;
	}

	vOut.Position.z = -vOut.Position.z; 

	vOut.FColor = vIn.Color;
	vOut.FTexturingType = vIn.TexturingType;

	vOut.FTexCoord0 = vIn.TexCoord0;
	vOut.FTexCoord1 = vIn.TexCoord1;
	
	vOut.FTex0Weight= vIn.TexWeight0;
	
    return vOut;
}
