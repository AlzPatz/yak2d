#version 330 core

uniform PixelSizeFactor
{
    vec2 PixelShift;
    vec2 Pad0;
    vec4 Pad1;
};

uniform sampler2D SamplerHeightMap;

in vec2 FTex;

out vec2 fragColor;

void main(void)
{
    float right = texture(SamplerHeightMap, FTex + vec2(PixelShift.x, 0.0)).r;
    float left = texture(SamplerHeightMap, FTex - vec2(PixelShift.x, 0.0)).r;
    
    float top = texture(SamplerHeightMap, FTex + vec2(0.0, PixelShift.y)).r;
    float bottom = texture(SamplerHeightMap, FTex - vec2(0.0f, PixelShift.y)).r;

    float centre = texture(SamplerHeightMap, FTex).r;

    float horizontal = 0.5 * ((centre - left) + (right - centre));
    float vertical = 0.5 * ((centre - bottom) + (top - centre)); 
    
    fragColor = vec2(horizontal, vertical);
}