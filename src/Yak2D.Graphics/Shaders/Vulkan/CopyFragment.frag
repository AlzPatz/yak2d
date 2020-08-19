#version 450

layout(set = 0, binding = 0) uniform texture2D Texture;
layout(set = 0, binding = 1) uniform sampler Sampler;

layout(location = 0) in vec2 FTex;
layout(location = 0) out vec4 fragColor;

void main()
{
    fragColor = texture(sampler2D(Texture, Sampler), FTex);
}