#version 330 core

uniform DistortionFactor
{
    vec2 DistortionScalar;
    vec2 Pad2;
    vec2 Pad3;
};

uniform sampler2D SamplerGradientMap;
uniform sampler2D SamplerImage;

in vec2 FTex;

out vec4 fragColor;

void main(void)
{
    float x = texture(SamplerGradientMap, FTex).r;
    float y = texture(SamplerGradientMap, FTex).g;
    vec2 pixelShift = DistortionScalar * vec2(x, y);
    fragColor = texture(SamplerImage, FTex + pixelShift);
}