#version 330 core

//Requires linear sampling of input texture
//http://rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/

#define ARRAY_SIZE 8

uniform sampler2D Sampler_Source; 

uniform LinearSampledGaussianUniforms
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

uniform WeightsAndOffsets
{
	Factor[ARRAY_SIZE] Factors;
};

in vec2 FTex;
out vec4 fragColor;

void main()
{
	vec4 colour = texture(Sampler_Source, FTex) * Factors[0].Weight;

	for(int n = 1; n < NumberOfSamples; n++)
	{
		colour += texture(Sampler_Source, FTex + (Factors[n].Offset * TexelShiftSize)) * Factors[n].Weight;
		colour += texture(Sampler_Source, FTex - (Factors[n].Offset * TexelShiftSize)) * Factors[n].Weight;
	}	

	fragColor = colour;
}