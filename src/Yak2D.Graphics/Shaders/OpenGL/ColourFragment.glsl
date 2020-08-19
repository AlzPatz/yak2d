#version 330 core

uniform Factors
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

uniform sampler2D Sampler;

in vec2 FTex;
out vec4 fragColor;

void main()
{
    vec4 sample = texture(Sampler, FTex);

    if(SingleColourAmount > 0.0)
    {
        vec4 single = Colour;

        if(sample.a == 0.0)
        {
            single = vec4(0.0, 0.0, 0.0, 0.0);
        }

        sample = mix(sample, single, SingleColourAmount);
    }

    vec4 grayScaled;

    if(GrayScaleAmount > 0.0)
    {
        float comp =    (sample.r * 0.299) + 
                        (sample.g * 0.587) +
                        (sample.b * 0.114);

        grayScaled = vec4(comp, comp, comp, sample.a);

        sample = mix(sample, grayScaled, GrayScaleAmount);
    }

    if(ColouriseAmount > 0.0)
    {
        vec4 modifiy = sample;
        vec4 mixColour = Colour;

        mixColour.rgb = clamp(mixColour.rgb / mixColour.a, 0, 1);
        if (modifiy.r > 0.5) modifiy.r = 1 - (1 - 2 * (modifiy.r - 0.5)) * (1 - mixColour.r); else modifiy.r = (2 * modifiy.r) * mixColour.r;
        if (modifiy.g > 0.5) modifiy.g = 1 - (1 - 2 * (modifiy.g - 0.5)) * (1 - mixColour.g); else modifiy.g = (2 * modifiy.g) * mixColour.g;
        if (modifiy.b > 0.5) modifiy.b = 1 - (1 - 2 * (modifiy.b - 0.5)) * (1 - mixColour.b); else modifiy.b = (2 * modifiy.b) * mixColour.b;
        
        sample = mix(sample, modifiy, ColouriseAmount);
    }

    if(NegativeAmount > 0.0)
    {
        vec4 negative = vec4(1.0 - sample.r, 1.0 - sample.g, 1.0- sample.b, sample.a); 
        sample = mix(sample, negative, NegativeAmount);
    }

    sample *= Opacity;

    if(sample.a == 0.0)
    {
        discard;
    }

    fragColor = sample;
}