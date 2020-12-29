#version 450

layout(set = 0, binding = 0) uniform DistortionFactor
{
    vec2 DistortionScalar;
    vec2 Pad2;
    vec4 Pad3;
};

layout(set = 1, binding = 0) uniform texture2D TextureGradientMap;
layout(set = 1, binding = 1) uniform sampler SamplerGradientMap;

layout(set = 2, binding = 0) uniform texture2D TextureImage;
layout(set = 2, binding = 1) uniform sampler SamplerImage;

layout(location = 0) in vec2 FTex;
layout(location = 0) out vec4 fragColor;

void main(void)
{
    float x = texture(sampler2D(TextureGradientMap, SamplerGradientMap), FTex).r;
    float y = texture(sampler2D(TextureGradientMap, SamplerGradientMap), FTex).g;
    vec2 pixelShift = DistortionScalar * vec2(x, y);
    fragColor = texture(sampler2D(TextureImage, SamplerImage), FTex + pixelShift);
}