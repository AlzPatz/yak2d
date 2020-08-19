#version 450

#define ARRAY_SIZE 16

layout(set = 0, binding = 0) uniform texture2D Texture; 
layout(set = 0, binding = 1) uniform sampler Sampler; 

layout(set = 1, binding = 0) uniform SampleUniforms
{
	vec2 TexelSize;
	int NumberOfSamples;
	float Pad0;
	vec4 Pad1;
};

struct Factor
{
	vec2 Offset;
	float Weight;
	float Pad;
};

layout(set = 2, binding = 0) uniform WeightsAndOffsets
{
	Factor[ARRAY_SIZE] Factors;
};

layout(location = 0) in vec2 FTex;
layout(location = 0) out vec4 fragColor;

void main()
{
    vec4 sum = vec4(0);

    for(int i = 0; i < NumberOfSamples; i++)
    {
        sum += Factors[i].Weight * texture(sampler2D(Texture, Sampler), FTex + (Factors[i].Offset * TexelSize));
    }

    fragColor = sum;
}