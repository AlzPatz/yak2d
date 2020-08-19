#version 450

layout(set = 0, binding = 0) uniform UniformBlock
{
	float MixAmount;
	float Pad0;
	float Pad1;
	float Pad2;
	vec4 Pad3;
};

layout(set = 1, binding = 0) uniform texture2D Texture_Source;
layout(set = 1, binding = 1) uniform sampler Sampler_Source;

layout(set = 2, binding = 0) uniform texture2D Texture_Processed;
layout(set = 2, binding = 1) uniform sampler Sampler_Processed;

layout(location = 0) in vec2 FTex;
layout(location = 0) out vec4 fragColor;

void main()
{
	vec4 source = texture(sampler2D(Texture_Source, Sampler_Source), FTex);
	vec4 processed = texture(sampler2D(Texture_Processed, Sampler_Processed), FTex);

	fragColor = source + (MixAmount * processed);
}