#version 330 core

#define ARRAY_SIZE 16

uniform sampler2D Sampler; 

uniform SampleUniforms
{
	vec2 TexelSize;
	int NumberOfSamples;
	float BrightnessThreshold;
	vec4 Pad0;
};

struct Factor
{
	vec2 Offset;
	float Weight;
	float Pad;
};

uniform WeightsAndOffsets
{
	Factor[ARRAY_SIZE] Factors;
};

in vec2 FTex;
out vec4 fragColor;

void main()
{
    vec4 sum = vec4(0);

    for(int i = 0; i < NumberOfSamples; i++)
    {
        sum += Factors[i].Weight * texture(Sampler, FTex + (Factors[i].Offset * TexelSize));
    }

    float brightness = dot(sum.rgb, vec3(0.2126, 0.7152, 0.0722));
    if(brightness  < BrightnessThreshold)
    {
        sum = vec4(0);
    }

    fragColor = sum;
}