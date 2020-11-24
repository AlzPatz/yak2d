SamplerState Sampler_Source : register(s0);
Texture2D Texture_Source : register(t0);

SamplerState Sampler_Noise : register(s1);
Texture2D Texture_Noise : register(t1);

SamplerState Sampler_ScanlineMask : register(s2);
Texture2D Texture_ScanlineMask : register(t2);

cbuffer PixellateFactors  : register(b0)
{
	float PixAmount;
	int NumXDivisions;
	int NumYDivisions;
	float Pad0;
	float2 TexelSize;
	float2 Pad1;
};

cbuffer EdgeDetectionFactors  : register(b1)
{
	int DetectEdges;
	int IsFreichen;
	float EdgeAmount;
	float4 Pad2;
};

cbuffer StaticFactors  : register(b2)
{
	float StaticAmount;
	float Time;
	int IgnoreTransparent;
	float TexelScaler;
	float4 Pad3;
};

cbuffer OldMovieFactors  : register(b3)
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

cbuffer CrtEffectFactors  : register(b4)
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

static float3x3 G[2] = {
	float3x3(1., 2., 1., 0., 0., 0., -1., -2., -1.),
	float3x3(1., 0., -1., 2., 0., -2., 1., 0., -1.)
};

static float3x3 H[9] = {
	float3x3(1. / (2.*sqrt(2.))*float3x3(1., sqrt(2.), 1., 0., 0., 0., -1., -sqrt(2.), -1.)),
	float3x3(1. / (2.*sqrt(2.))*float3x3(1., 0., -1., sqrt(2.), 0., -sqrt(2.), 1., 0., -1.)),
	float3x3(1. / (2.*sqrt(2.))*float3x3(0., -1., sqrt(2.), 1., 0., -1., -sqrt(2.), 1., 0.)),
	float3x3(1. / (2.*sqrt(2.))*float3x3(sqrt(2.), -1., 0., -1., 0., 1., 0., 1., -sqrt(2.))),
	float3x3(1. / 2.*float3x3(0., 1., 0., -1., 0., -1., 0., 1., 0.)),
	float3x3(1. / 2.*float3x3(-1., 0., 1., 0., 0., 0., 1., 0., -1.)),
	float3x3(1. / 6.*float3x3(1., -2., 1., -2., 4., -2., 1., -2., 1.)),
	float3x3(1. / 6.*float3x3(-2., 1., -2., 1., 4., 1., -2., 1., -2.)),
	float3x3(1. / 3.*float3x3(1., 1., 1., 1., 1., 1., 1., 1., 1.))
};

float4 grayscale(float4 col)
{
	float comp = (col.r*.299) +
		(col.g*.587) +
		(col.b*.114);

	return float4(comp, comp, comp, col.a);
}

float2 ModifyTexCoords(float2 tex)
{
	float2 modified = tex;

	if (OldMovieAmount > 0.0)
	{
		float2 movieTex = modified;
		float2 mrnd;
		float shift;
		float yshift = 0.;
		yshift = CpuShift;
		if (yshift < 0.) {
			yshift += 1.;
		}
		mrnd = Frame * float2(.003, .06);
		shift = Texture_Noise.Sample(Sampler_Noise, mrnd).r;
		if (shift > RndShiftCutOff)
		{
			yshift = (-1. + (2.*shift))*RndShiftScalar;
		}
		movieTex.y += yshift; //Invert versus OpenGL
		modified = lerp(modified, movieTex, OldMovieAmount);
	}

	if (PixAmount > 0.0)
	{
		float2 pixTex;
		float dx = 1. / (1.*NumXDivisions);
		float dy = 1. / (1.*NumYDivisions);
		int sx = int(modified.x / dx);
		int sy = int(modified.y / dy);
		pixTex.x = float(sx)*dx;
		pixTex.y = float(sy)*dy;
		modified = lerp(modified, pixTex, PixAmount);
	}

	return modified;
}

float4 ProcessEdgeDetection(float2 tex)
{
	if (IsFreichen)
	{
		float3x3 I;
		float cnv[9];
		float3 sample;

		/* fetch the 3x3 neighbourhood and use the RGB vector's length as intensity value */
		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++) {
				sample = grayscale(Texture_Source.Sample(Sampler_Source, tex + float2((i - 1) * TexelSize.x, (j - 1) * TexelSize.y))).rgb;
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
				sample = grayscale(Texture_Source.Sample(Sampler_Source, tex + float2((l - 1) * TexelSize.x, (l - 1) * TexelSize.y))).rgb;
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
	return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
}

float4 ProcessStatic(float4 colour, float2 tex)
{
	float2 scaledTexSize = TexelScaler * TexelSize;
	float2 pixel = tex / scaledTexSize;
	float2 pixInt = float2(floor(pixel.x), floor(pixel.y));
	float rnd = rand(Time * (pixInt + 1.376)); //Shift forward pixInt to stop origin pixel sticking on single colour
	float4 result = float4(rnd, rnd, rnd, 1.0);
	float mask = clamp(colour.a + IgnoreTransparent, 0.0, 1.0);
	return mask * result;
}

float4 ProcessOldMovieColour(float4 colour, float2 tex)
{
	float2 rndx;
	float dim;
	float2 sc;
	float scr;
	float2 rLine;
	float3 rand;
	rndx = Frame * float2(.007, .01);
	dim = Texture_Noise.Sample(Sampler_Noise, rndx).r;
	dim *= Dim;
	if (dim > 1.) {
		dim = 1.;
	}
	dim = 1. - dim;
	colour.rgb *= dim;
	sc = Frame * float2(.01, .04);
	sc.x = frac(tex.x + sc.x);
	scr = Texture_Noise.Sample(Sampler_Noise, sc).r;
	scr = 2.*scr*(1. / Scratch);
	scr = 1. - abs(1. - scr);
	scr = max(0., scr);
	colour.rgb += scr;
	rLine = (tex + RndLine1)*.33;
	rand = Texture_Noise.Sample(Sampler_Noise, rLine).rgb;
	if (Noise > rand.r) {
		float nse = .1 + rand.b*.4;
		colour.rgb = float3(nse, nse, nse);
	}

	float2 dist;
	float flux;
	colour = float4(.9*colour.r, .8*colour.g, .6*colour.b, colour.a);
	dist = .5 - tex;
	flux = RndLine2 * .04 - .02;
	colour.rgb *= (.4 + flux - dot(dist, dist))*2.8;
	colour *= OverExposureColour;

	return colour;
}

float4 ProcessRgbFilter(float4 current, float2 tex)
{
	float neg = 1. - RgbFilterIntensity;
	float4 modified = current * neg;
	//We multiple by three as we are roughly filtering each pixel through one of 3 colours so expect about 2/3+ of all colour intensity to be lost across the image
	float4 filtered = 3.*current*RgbFilterIntensity*Texture_ScanlineMask.Sample(Sampler_ScanlineMask, float2(tex.x*NumRgbFiltersHorizontal, tex.y*NumRgbFiltersVertical));
	modified += filtered;
	return modified;
}

struct FragmentIn
{
	float4 Position : SV_Position;
	float2 FTex : TEXCOORD;
};

float4 main(FragmentIn fIn) : SV_Target
{
	float2 tex = ModifyTexCoords(fIn.FTex);

	float4 sample = Texture_Source.Sample(Sampler_Source, tex);

	if (DetectEdges)
	{
		sample = lerp(sample, ProcessEdgeDetection(tex), EdgeAmount);
	}

	if (StaticAmount > 0.0)
	{
		sample = lerp(sample, ProcessStatic(sample, tex), StaticAmount);
	}

	if (OldMovieAmount > 0.0)
	{
		sample = lerp(sample, ProcessOldMovieColour(sample, tex), OldMovieAmount);
	}

	if (RgbFilterAmount > 0.0)
	{
		sample = lerp(sample, ProcessRgbFilter(sample, tex), RgbFilterAmount);
	}

	if (ScanLineAmount > 0.0)
	{
		float ty = 2.0 * TexelSize.y; //Hard coded simple scaline sizing
		sample = sample * (fmod(tex.y, ty * 2.0) * (1.0 / ty));
	}

	return sample;
}