#include <metal_stdlib>
using namespace metal;

struct PixellateFactors  
{
	float PixAmount;
	int NumXDivisions;
	int NumYDivisions;
	float Pad0;
	float2 TexelSize;
	float2 Pad1;
};

struct EdgeDetectionFactors  
{
	int DetectEdges;
	int IsFreichen;
	float EdgeAmount;
	float4 Pad2;
};

struct StaticFactors  
{
	float StaticAmount;
	float Time;
	int IgnoreTransparent;
	float TexelScaler;
	float4 Pad3;
};

struct OldMovieFactors  
{
	float OldMovieAmount;
	float Scratch;
	float Noise;
	float RndLine1;
	float RndLine2;
	float Frame;
	float CpuShift;
	float RndShiftCutOff;
	float RndShiftScalar;
	float Dim;
	float2 Pad4;
	float4 OverExposureColour;
};

struct CrtEffectFactors  
{
	float RgbFilterIntensity;
	float RgbFilterAmount;
	int NumRgbFiltersHorizontal;
	int NumRgbFiltersVertical;
	float ScanLineAmount;
	float Pad5;
	float Pad6;
	float Pad7;
};

struct FragmentIn
{
    float4 Position [[attribute(0)]];
    float2 FTex [[attribute(1)]];
};

float4 grayscale(float4 col)
{
	float comp = (col.r*.299) +
		(col.g*.587) +
		(col.b*.114);

	return float4(comp, comp, comp, col.a);
}

float2 ModifyTexCoords( float2 tex,
                        constant OldMovieFactors &oldMovieFactors,
                        constant PixellateFactors &pixellateFactors,
                        texture2d<float, access::sample> Texture_Noise,
                        sampler Sampler_Noise
                        )
{
	float2 modified = tex;

	if (oldMovieFactors.OldMovieAmount > 0.0)
	{
		float2 movieTex = modified;
		float2 mrnd;
		float shift;
		float yshift = 0.;
		yshift = oldMovieFactors.CpuShift;
		if (yshift < 0.) {
			yshift += 1.;
		}
		mrnd = oldMovieFactors.Frame * float2(.003, .06);
		shift = Texture_Noise.sample(Sampler_Noise, mrnd).r;
		if (shift > oldMovieFactors.RndShiftCutOff)
		{
			yshift = (-1. + (2.*shift))*oldMovieFactors.RndShiftScalar;
		}
		movieTex.y += yshift; //Invert versus OpenGL
		modified = mix(modified, movieTex, oldMovieFactors.OldMovieAmount);
	}

	if (pixellateFactors.PixAmount > 0.0)
	{
		float2 pixTex;
		float dx = 1. / (1.*pixellateFactors.NumXDivisions);
		float dy = 1. / (1.*pixellateFactors.NumYDivisions);
		int sx = int(modified.x / dx);
		int sy = int(modified.y / dy);
		pixTex.x = float(sx)*dx;
		pixTex.y = float(sy)*dy;
		modified = mix(modified, pixTex, pixellateFactors.PixAmount);
	}

	return modified;
}

//Constructed in Column Major hence Y and X values appear flipped versus other backends
constant float3x3 G[2] = { 
	float3x3(float3(1.,0.,-1.), float3(2.,0.,-2.), float3(1.,0.,-1.)),
	float3x3(float3(1.,2.,1.), float3(0.,0.,0.), float3(-1.,-2.,-1.))
};

//Constructed in Column Major hence Y and X values appear flipped versus other backends
constant float3x3 H[9] = {
        1. / (2*1.41421) * float3x3(float3(1, 0, -1) , float3(1.41421, 0, -1.41421) , float3(1, 0, -1)),
        1. / (2*1.41421) * float3x3(float3(1, 1.41421, 1) , float3( 0, 0, 0) , float3( -1, -1.41421, -1)),
        1. / (2*1.41421) * float3x3(float3(0, 1, -1.41421) , float3( -1, 0, 1) , float3( 1.41421, -1, 0)),
        1. / (2*1.41421) * float3x3(float3(1.41421, -1, 0) , float3( -1, 0, 1) , float3( 0, 1, -1.41421)),
        1. / 2. * float3x3(float3(0, -1, 0) , float3( 1, 0, 1) , float3( 0, -1, 0)),
        1. / 2. * float3x3(float3(-1, 0, 1) , float3( 0, 0, 0) ,float3( 1, 0, -1)),
        1. / 6. * float3x3(float3(1, -2, 1) , float3( -2, 4, -2) , float3( 1, -2, 1)),
        1. / 6. * float3x3(float3(-2, 1, -2) ,float3( 1, 4, 1) , float3( -2, 1, -2)),
       	1. / 3. * float3x3(float3(1, 1, 1) , float3( 1, 1, 1) , float3( 1, 1, 1))
};

float4 ProcessEdgeDetection(    float2 tex,
                                constant EdgeDetectionFactors &edgeDetectionFactors,
                                constant PixellateFactors &pixellateFactors,
                                texture2d<float, access::sample> Texture_Source,
                                sampler Sampler_Source)
{
	if (edgeDetectionFactors.IsFreichen)
	{
		float3x3 I;
		float cnv[9];
		float3 sample;

		/* fetch the 3x3 neighbourhood and use the RGB vector's length as intensity value */
		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++) {
				sample = grayscale(Texture_Source.sample(Sampler_Source, tex + float2((i - 1) * pixellateFactors.TexelSize.x, (j - 1) * pixellateFactors.TexelSize.y))).rgb;
				I[i][j] = length(sample);
			}

		/* calculate the convolution values for all the masks */
		for (int k = 0; k < 9; k++) {
			float dp3 = dot(H[k][0], I[0]) + dot(H[k][1], I[1]) + dot(H[k][2], I[2]);
			cnv[k] = dp3 * dp3;
		}

		float M = (cnv[0] + cnv[1]) + (cnv[2] + cnv[3]);
		float S = (cnv[4] + cnv[5]) + (cnv[6] + cnv[7]) + (cnv[8] + M);

		float val = sqrt(M / S);
		return float4(val, val, val , val);
	}
	else
	{
		float3x3 I;
		float cnv[2];
		float3 sample;

		/* fetch the 3x3 neighbourhood and use the RGB vector's length as intensity value */
		for (int l = 0; l < 3; l++)
			for (int j = 0; j < 3; j++) {
				sample = grayscale(Texture_Source.sample(Sampler_Source, tex + float2((l - 1) * pixellateFactors.TexelSize.x, (l - 1) * pixellateFactors.TexelSize.y))).rgb;
				I[l][j] = length(sample);
			}

		/* calculate the convolution values for all the masks */
		for (int p = 0; p < 2; p++) {
			float dp3 = dot(G[p][0], I[0]) + dot(G[p][1], I[1]) + dot(G[p][2], I[2]);
			cnv[p] = dp3 * dp3;
		}

		float value = .5*sqrt(cnv[0] * cnv[0] + cnv[1] * cnv[1]);
		return float4(value, value, value, value);
	}
}

float rand(float2 co) {
	return fract(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
}

float4 ProcessStatic(float4 colour, float2 tex, constant StaticFactors &staticFactors, constant PixellateFactors &pixellateFactors)
{
	float2 scaledTexSize = staticFactors.TexelScaler * pixellateFactors.TexelSize;
	float2 pixel = tex / scaledTexSize;
	float2 pixInt = float2(floor(pixel.x), floor(pixel.y));
	float rnd = rand(staticFactors.Time * (pixInt + 1.376)); //Shift forward pixInt to stop origin pixel sticking on single colour
	float4 result = float4(rnd, rnd, rnd, 1.0);
	float mask = clamp(colour.a + staticFactors.IgnoreTransparent, 0.0, 1.0);
	return mask * result;
}

float4 ProcessOldMovieColour(   float4 colour, float2 tex, constant OldMovieFactors &oldMovieFactors,
                                texture2d<float, access::sample> Texture_Noise,
                                sampler Sampler_Noise)
{
	float2 rndx;
	float dim;
	float2 sc;
	float scr;
	float2 rLine;
	float3 rand;
	rndx = oldMovieFactors.Frame * float2(.007, .01);
	dim = Texture_Noise.sample(Sampler_Noise, rndx).r;
	dim *= oldMovieFactors.Dim;
	if (dim > 1.) {
		dim = 1.;
	}
	dim = 1. - dim;
	colour.rgb *= dim;
	sc = oldMovieFactors.Frame * float2(.01, .04);
	sc.x = fract(tex.x + sc.x);
	scr = Texture_Noise.sample(Sampler_Noise, sc).r;
	scr = 2.*scr*(1. / oldMovieFactors.Scratch);
	scr = 1. - abs(1. - scr);
	scr = max(0., scr);
	colour.rgb += scr;
	rLine = (tex + oldMovieFactors.RndLine1)*.33;
	rand = Texture_Noise.sample(Sampler_Noise, rLine).rgb;
	if (oldMovieFactors.Noise > rand.r) {
		float nse = .1 + rand.b*.4;
		colour.rgb = float3(nse, nse, nse);
	}

	float2 dist;
	float flux;
	colour = float4(.9*colour.r, .8*colour.g, .6*colour.b, colour.a);
	dist = .5 - tex;
	flux = oldMovieFactors.RndLine2 * .04 - .02;
	colour.rgb *= (.4 + flux - dot(dist, dist))*2.8;
	colour *= oldMovieFactors.OverExposureColour;

	return colour;
}

float4 ProcessRgbFilter(float4 current, float2 tex, constant CrtEffectFactors &crtEffectFactors,
                    texture2d<float, access::sample> Texture_ScanlineMask,
                    sampler Sampler_ScanlineMask)
{
	float neg = 1. - crtEffectFactors.RgbFilterIntensity;
	float4 modified = current * neg;
	//We multiple by three as we are roughly filtering each pixel through one of 3 colours so expect about 2/3+ of all colour intensity to be lost across the image
	float4 filtered = 3.*current*crtEffectFactors.RgbFilterIntensity*Texture_ScanlineMask.sample(Sampler_ScanlineMask, float2(tex.x*crtEffectFactors.NumRgbFiltersHorizontal, tex.y*crtEffectFactors.NumRgbFiltersVertical));
	modified += filtered;
	return modified;
}

fragment float4 shader( FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> Texture_Source[[ texture(0)]],
                    sampler Sampler_Source [[ sampler(0)]],
                    texture2d<float, access::sample> Texture_Noise[[ texture(1)]],
                    sampler Sampler_Noise [[ sampler(1)]],
                    texture2d<float, access::sample> Texture_ScanlineMask [[ texture(2)]],
                    sampler Sampler_ScanlineMask [[ sampler(2)]],
                    constant PixellateFactors &pixellateFactors [[buffer(0)]],
                    constant EdgeDetectionFactors &edgeDetectionFactors [[buffer(1)]],
                    constant StaticFactors &staticFactors [[buffer(2)]],
                    constant OldMovieFactors &oldMovieFactors [[buffer(3)]],
                    constant CrtEffectFactors &crtEffectFactors [[buffer(4)]])
                    {
                        float2 tex = ModifyTexCoords(   fIn.FTex, 
                                                        oldMovieFactors,
                                                        pixellateFactors,
                                                        Texture_Noise,
                                                        Sampler_Noise);

                        float4 sample = Texture_Source.sample(Sampler_Source, tex);

                        if (edgeDetectionFactors.DetectEdges) 
                        {
                            sample = mix(   sample, 
                                            ProcessEdgeDetection(tex, edgeDetectionFactors, pixellateFactors, Texture_Source, Sampler_Source), 
                                            edgeDetectionFactors.EdgeAmount);
                        }

                        if (staticFactors.StaticAmount > 0.0)
                        {
                            sample = mix(sample, 
                                        ProcessStatic(sample, tex, staticFactors, pixellateFactors), 
                                        staticFactors.StaticAmount);
                        }

                        if (oldMovieFactors.OldMovieAmount > 0.0)
                        {
                            sample = mix(sample, ProcessOldMovieColour(sample, tex, oldMovieFactors, Texture_Noise, Sampler_Noise), oldMovieFactors.OldMovieAmount);
                        }

                        if (crtEffectFactors.RgbFilterAmount > 0.0)
                        {
                            sample = mix(sample, ProcessRgbFilter(sample, tex, crtEffectFactors, Texture_ScanlineMask, Sampler_ScanlineMask), crtEffectFactors.RgbFilterAmount);
                        }

                        if (crtEffectFactors.ScanLineAmount > 0.0)
                        {
                            sample = mix(sample, sample*fmod(tex.y, pixellateFactors.TexelSize.y*2.)*(1. / pixellateFactors.TexelSize.y), crtEffectFactors.ScanLineAmount);
                        }

                        return sample;
                    }