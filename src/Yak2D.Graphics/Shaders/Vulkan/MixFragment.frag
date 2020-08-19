#version 450

layout(set = 0, binding = 0) uniform MixFactors
{
	vec4 Amount;
}; 

layout(set = 1, binding = 0) uniform texture2D TextureMix; 
layout(set = 1, binding = 1) uniform sampler SamplerMix; 

layout(set = 2, binding = 0) uniform texture2D Texture0; 
layout(set = 2, binding = 1) uniform sampler Sampler0; 

layout(set = 3, binding = 0) uniform texture2D Texture1; 
layout(set = 3, binding = 1) uniform sampler Sampler1; 

layout(set = 4, binding = 0) uniform texture2D Texture2; 
layout(set = 4, binding = 1) uniform sampler Sampler2; 

layout(set = 5, binding = 0) uniform texture2D Texture3; 
layout(set = 5, binding = 1) uniform sampler Sampler3; 

layout(location = 0) in vec2 FTex;
layout(location = 0) out vec4 fragColor;

void main()
{
	vec4 mixing = texture(sampler2D(TextureMix, SamplerMix), FTex);
	
	fragColor =	  	(Amount.r * mixing.r * texture(sampler2D(Texture0, Sampler0), FTex)) + 
					(Amount.g * mixing.g * texture(sampler2D(Texture1, Sampler1), FTex)) + 
    				(Amount.b * mixing.b * texture(sampler2D(Texture2, Sampler2), FTex)) + 
    				(Amount.a * mixing.a * texture(sampler2D(Texture3, Sampler3), FTex));
}