#include <metal_stdlib>
using namespace metal;

struct Factors
{
    float SingleColourAmount;
    float GrayScaleAmount;
    float ColouriseAmount;
    float NegativeAmount;
    float4 Colour;
    float Opacity;	
    float Pad0;
    float Pad1;
    float Pad2;
    float4 Pad3;
};

struct FragmentIn
{
    float4 Position [[attribute(0)]];
    float2 FTex [[attribute(1)]];
};

fragment float4 shader( FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> Texture [[ texture(0)]],
                    sampler Sampler [[ sampler(0)]],
                    constant Factors &factors [[buffer(0)]])
                    {
                        float4 sample = Texture.sample(Sampler, fIn.FTex);

                            if(factors.SingleColourAmount > 0.0)
                            {
                                float4 single = factors.Colour;

                                if(sample.a == 0.0)
                                {
                                        single = float4(0.0, 0.0, 0.0, 0.0);
                                }

                                sample = mix(sample, single, factors.SingleColourAmount);
                            }

                            float4 grayScaled;

                            if(factors.GrayScaleAmount > 0.0)
                            {
                                float comp =    (sample.r * 0.299) + 
                                                (sample.g * 0.587) +
                                                (sample.b * 0.114);

                                grayScaled = float4(comp, comp, comp, sample.a);

                                sample = mix(sample, grayScaled, factors.GrayScaleAmount);
                            }

                            if(factors.ColouriseAmount > 0.0)
                            {
                                float4 modifiy = sample;
                                float4 mixColour = factors.Colour;

                                mixColour.rgb = saturate(mixColour.rgb / mixColour.a);
                                if (modifiy.r > 0.5) modifiy.r = 1 - (1 - 2 * (modifiy.r - 0.5)) * (1 - mixColour.r); else modifiy.r = (2 * modifiy.r) * mixColour.r;
                                if (modifiy.g > 0.5) modifiy.g = 1 - (1 - 2 * (modifiy.g - 0.5)) * (1 - mixColour.g); else modifiy.g = (2 * modifiy.g) * mixColour.g;
                                if (modifiy.b > 0.5) modifiy.b = 1 - (1 - 2 * (modifiy.b - 0.5)) * (1 - mixColour.b); else modifiy.b = (2 * modifiy.b) * mixColour.b;
                            
                                sample = mix(sample, modifiy, factors.ColouriseAmount);
                            }

                            if(factors.NegativeAmount > 0.0)
                            {
                                float4 negative = float4(1.0 - sample.r, 1.0 - sample.g, 1.0- sample.b, sample.a); 
                                sample = mix(sample, negative, factors.NegativeAmount);
                            }

                            sample *= factors.Opacity;

                            if(sample.a == 0.0)
                            {
                                discard_fragment(); //Needed or fast, I forget?
                            }

                            return sample;
                    }