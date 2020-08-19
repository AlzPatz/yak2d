#version 450

layout(set = 0, binding = 0) uniform PixelSizeFactor
{
    vec2 PixelShift;
    vec2 Pad0;
    vec4 Pad1;
};

layout(set = 1, binding = 0) uniform texture2D TextureHeightMap;
layout(set = 1, binding = 1) uniform sampler SamplerHeightMap;

layout(location = 0) in vec2 FTex;
layout(location = 0) out vec2 fragColor;

void main(void)
{
    float right = texture(sampler2D(TextureHeightMap, SamplerHeightMap), FTex + vec2(PixelShift.x, 0.0)).r;
    float left = texture(sampler2D(TextureHeightMap, SamplerHeightMap), FTex - vec2(PixelShift.x, 0.0)).r;
    
    float top = texture(sampler2D(TextureHeightMap, SamplerHeightMap), FTex + vec2(0.0, PixelShift.y)).r;
    float bottom = texture(sampler2D(TextureHeightMap, SamplerHeightMap), FTex - vec2(0.0f, PixelShift.y)).r;

    float centre = texture(sampler2D(TextureHeightMap, SamplerHeightMap), FTex).r;

    float horizontal = 0.5 * ((centre - left) + (right - centre));
    float vertical = 0.5 * ((centre - bottom) + (top - centre)); 
    
    fragColor = vec2(horizontal, vertical);
}