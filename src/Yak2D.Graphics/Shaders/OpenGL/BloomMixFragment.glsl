#version 330 core

uniform UniformBlock
{
	float MixAmount;
	float Pad0;
	float Pad1;
	float Pad2;
	vec4 Pad3;
};

uniform sampler2D Sampler_Source;
uniform sampler2D Sampler_Processed;

in vec2 FTex;
out vec4 fragColor;

void main()
{
	vec4 source = texture(Sampler_Source, FTex);
	vec4 processed = texture(Sampler_Processed, FTex);

	fragColor = source + (MixAmount * processed);
}