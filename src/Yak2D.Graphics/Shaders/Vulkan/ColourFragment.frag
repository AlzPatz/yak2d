#version 450

layout(set = 0, binding = 0) uniform Factors
{
    float SingleColourAmount;
    float GrayScaleAmount;
    float ColouriseAmount;
    float NegativeAmount;
    vec4 Colour;
    float Opacity;
    float Pad0;
    float Pad1;
    float Pad2;
    vec4 Pad3;
};

layout(set = 1, binding = 0) uniform texture2D Texture;
layout(set = 1, binding = 1) uniform sampler Sampler;

layout(location = 0) in vec2 FTex;
layout(location = 0) out vec4 fragColor;

void main()
{
    vec4 col = texture(sampler2D(Texture, Sampler), FTex);

    if(SingleColourAmount > 0.0)
    {
        vec4 single = Colour;

        if(col.a == 0.0)
        {
            single = vec4(0.0, 0.0, 0.0, 0.0);
        }

        col = mix(col, single, SingleColourAmount);
    }

    vec4 grayScaled;

    if(GrayScaleAmount > 0.0)
    {
        float comp =    (col.r * 0.299) + 
                        (col.g * 0.587) +
                        (col.b * 0.114);

        grayScaled = vec4(comp, comp, comp, col.a);

        col = mix(col, grayScaled, GrayScaleAmount);
    }

    if(ColouriseAmount > 0.0)
    {
        vec4 modifiy = col;
        vec4 mixColour = Colour;

        mixColour.rgb = clamp(mixColour.rgb / mixColour.a, 0, 1);
        if (modifiy.r > 0.5) modifiy.r = 1 - (1 - 2 * (modifiy.r - 0.5)) * (1 - mixColour.r); else modifiy.r = (2 * modifiy.r) * mixColour.r;
        if (modifiy.g > 0.5) modifiy.g = 1 - (1 - 2 * (modifiy.g - 0.5)) * (1 - mixColour.g); else modifiy.g = (2 * modifiy.g) * mixColour.g;
        if (modifiy.b > 0.5) modifiy.b = 1 - (1 - 2 * (modifiy.b - 0.5)) * (1 - mixColour.b); else modifiy.b = (2 * modifiy.b) * mixColour.b;
        
        col = mix(col, modifiy, ColouriseAmount);
    }

    if(NegativeAmount > 0.0)
    {
        vec4 negative = vec4(1.0 - col.r, 1.0 - col.g, 1.0- col.b, col.a); 
        col = mix(col, negative, NegativeAmount);
    }

    col *= Opacity;

    if(col.a == 0.0)
    {
        discard;
    }

    fragColor = col;
}