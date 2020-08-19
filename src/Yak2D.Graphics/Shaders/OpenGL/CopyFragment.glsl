#version 330 core

uniform sampler2D Sampler; 

in vec2 FTex;
out vec4 fragColor;

void main()
{
    fragColor = texture(Sampler, FTex);
}