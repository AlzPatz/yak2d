#version 450

//Requires linear sampling of input texture
//http://rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/

#define ARRAY_SIZE 8

layout(set = 0, binding = 0) uniform texture2D Texture_Source; 
layout(set = 0, binding = 1) uniform sampler Sampler_Source; 

layout(set = 1, binding = 0) uniform LinearSampledGaussianUniforms
{
	vec2 TexelShiftSize;
	int NumberOfSamples;
	float Pad0;
	vec4 Pad1;
};

struct Factor
{
	float Offset;
	float Weight;
	vec2 Pad;
};

layout(set = 2, binding = 0) uniform WeightsAndOffsets
{
	Factor[ARRAY_SIZE] Factors;
};

layout(location = 0) in vec2 FTex;
layout(location = 0) out vec4 fragColor;

void main()
{
	vec4 colour = texture(sampler2D(Texture_Source, Sampler_Source), FTex) * Factors[0].Weight;

	
	for(int n = 1; n < NumberOfSamples; n++)
	{
		colour += texture(sampler2D(Texture_Source, Sampler_Source), FTex + (Factors[n].Offset * TexelShiftSize)) * Factors[n].Weight;
		colour += texture(sampler2D(Texture_Source, Sampler_Source), FTex - (Factors[n].Offset * TexelShiftSize)) * Factors[n].Weight;
	}	

	fragColor = colour;
}