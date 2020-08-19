#version 330 core

uniform MixFactors
{
	vec4 Amount;
}; 

uniform sampler2D SamplerMix;
uniform sampler2D Sampler0;
uniform sampler2D Sampler1;
uniform sampler2D Sampler2;
uniform sampler2D Sampler3;

in vec2 FTex;
out vec4 fragColor;

void main()
{
	vec4 mixing = texture(SamplerMix, FTex);
	
	fragColor =	  	(Amount.r * mixing.r * texture(Sampler0, FTex)) + 
					(Amount.g * mixing.g * texture(Sampler1, FTex)) + 
    				(Amount.b * mixing.b * texture(Sampler2, FTex)) + 
    				(Amount.a * mixing.a * texture(Sampler3, FTex));
}