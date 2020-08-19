cbuffer Factors : register(b0)
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


SamplerState Sampler : register(s0);
Texture2D Texture : register(t0);

struct FragmentIn
{
	float4 Position : SV_Position;
	float2 FTex : TEXCOORD;
};

float4 main(FragmentIn fIn) : SV_Target
{
    	/*
        We do not clamp all 0-1 fIns to the range assuming clean fIn. Perhaps check if this is
        relatively free / low cost
    	*/
    	
	float4 sample = Texture.Sample(Sampler, fIn.FTex);     

    	if(SingleColourAmount > 0.0)
    	{
        	float4 single = Colour;

        	if(sample.a == 0.0)
        	{
        	    	single = float4(0.0, 0.0, 0.0, 0.0);
 	       	}

        	sample = lerp(sample, single, SingleColourAmount);
    	}

    	float4 grayScaled;

    	if(GrayScaleAmount > 0.0)
    	{
        	float comp =    (sample.r * 0.299) + 
                        	(sample.g * 0.587) +
                        	(sample.b * 0.114);

        	grayScaled = float4(comp, comp, comp, sample.a);

        	sample = lerp(sample, grayScaled, GrayScaleAmount);
    	}

    	if(ColouriseAmount > 0.0)
    	{
        	float4 modifiy = sample;
        	float4 mixColour = Colour;

        	mixColour.rgb = clamp(mixColour.rgb / mixColour.a, 0, 1);
        	if (modifiy.r > 0.5) modifiy.r = 1 - (1 - 2 * (modifiy.r - 0.5)) * (1 - mixColour.r); else modifiy.r = (2 * modifiy.r) * mixColour.r;
        	if (modifiy.g > 0.5) modifiy.g = 1 - (1 - 2 * (modifiy.g - 0.5)) * (1 - mixColour.g); else modifiy.g = (2 * modifiy.g) * mixColour.g;
        	if (modifiy.b > 0.5) modifiy.b = 1 - (1 - 2 * (modifiy.b - 0.5)) * (1 - mixColour.b); else modifiy.b = (2 * modifiy.b) * mixColour.b;
        
        	sample = lerp(sample, modifiy, ColouriseAmount);
    	}

    	if(NegativeAmount > 0.0)
    	{
        	float4 negative = float4(1.0 - sample.r, 1.0 - sample.g, 1.0- sample.b, sample.a); 
        	sample = lerp(sample, negative, NegativeAmount);
    	}

    	sample *= Opacity;

    	if(sample.a == 0.0)
    	{
        	discard; //Needed or fast, I forget?
    	}

    	return sample;
}