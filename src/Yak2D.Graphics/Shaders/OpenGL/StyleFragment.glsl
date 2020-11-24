#version 330 core

uniform sampler2D Sampler_Source;
uniform sampler2D Sampler_Noise;
uniform sampler2D Sampler_ScanlineMask;

uniform PixellateFactors
{
	float PixAmount;
	int NumXDivisions;
	int NumYDivisions;
	float Pad0;
	vec2 TexelSize;
	vec2 Pad1;
};

uniform EdgeDetectionFactors
{
	int DetectEdges;
	int IsFreichen;
	float EdgeAmount;
	vec4 Pad2;
};

uniform StaticFactors
{
	float StaticAmount;
	float Time;
	int IgnoreTransparent;
	float TexelScaler;
	vec4 Pad3;
};

uniform OldMovieFactors
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
	vec2 Pad4;
	vec4 OverExposureColour;
};

uniform CrtEffectFactors
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

uniform mat3 G[2]=mat3[](
	mat3(1.,2.,1.,0.,0.,0.,-1.,-2.,-1.),
	mat3(1.,0.,-1.,2.,0.,-2.,1.,0.,-1.)
);

uniform mat3 H[9]=mat3[](
	1./(2.*sqrt(2.))*mat3(1.,sqrt(2.),1.,0.,0.,0.,-1.,-sqrt(2.),-1.),
	1./(2.*sqrt(2.))*mat3(1.,0.,-1.,sqrt(2.),0.,-sqrt(2.),1.,0.,-1.),
	1./(2.*sqrt(2.))*mat3(0.,-1.,sqrt(2.),1.,0.,-1.,-sqrt(2.),1.,0.),
	1./(2.*sqrt(2.))*mat3(sqrt(2.),-1.,0.,-1.,0.,1.,0.,1.,-sqrt(2.)),
	1./2.*mat3(0.,1.,0.,-1.,0.,-1.,0.,1.,0.),
	1./2.*mat3(-1.,0.,1.,0.,0.,0.,1.,0.,-1.),
	1./6.*mat3(1.,-2.,1.,-2.,4.,-2.,1.,-2.,1.),
	1./6.*mat3(-2.,1.,-2.,1.,4.,1.,-2.,1.,-2.),
	1./3.*mat3(1.,1.,1.,1.,1.,1.,1.,1.,1.)
);

in vec2 FTex;
out vec4 fragColor;

vec4 grayscale(vec4 col)
{
	float comp=(col.r*.299)+
	(col.g*.587)+
	(col.b*.114);
	
	return vec4(comp,comp,comp,col.a);
}

vec2 ModifyTexCoords(vec2 tex)
{
	vec2 modified=tex;
	
	if(OldMovieAmount > 0.0)
	{
		vec2 movieTex=modified;
		vec2 mrnd;
		float shift;
		float yshift=0.;
		yshift=CpuShift;
		if(yshift<0.){
			yshift+=1.;
		}
		mrnd=Frame*vec2(.003,.06);
		shift=texture(Sampler_Noise,mrnd).r;
		if(shift>RndShiftCutOff)
		{
			yshift=(-1.+(2.*shift))*RndShiftScalar;
		}
		movieTex.y-=yshift;
		modified=mix(modified,movieTex,	OldMovieAmount);
	}
	
	if(PixAmount > 0.0)
	{
		vec2 pixTex;
		float dx=1./(1.*NumXDivisions);
		float dy=1./(1.*NumYDivisions);
		int sx=int(modified.x/dx);
		int sy=int(modified.y/dy);
		pixTex.x=float(sx)*dx;
		pixTex.y=float(sy)*dy;
		modified=mix(modified,pixTex,PixAmount);
	}
	
	return modified;
}

vec4 ProcessEdgeDetection(vec2 tex)
{
	if(IsFreichen == 1)
	{
		mat3 I;
		float cnv[9];
		vec3 sample;
		
		/* fetch the 3x3 neighbourhood and use the RGB vector's length as intensity value */
		for (int i=0; i<3; i++)
		for (int j=0; j<3; j++) {
			sample = grayscale(texture(Sampler_Source, tex + vec2((i-1) * TexelSize.x, (j-1) * TexelSize.y))).rgb;
			I[i][j] = length(sample);
		}
		
		/* calculate the convolution values for all the masks */
		for (int i=0; i<9; i++) {
			float dp3 = dot(H[i][0], I[0]) + dot(H[i][1], I[1]) + dot(H[i][2], I[2]);
			cnv[i] = dp3 * dp3;
		}
		
		float M = (cnv[0] + cnv[1]) + (cnv[2] + cnv[3]);
		float S = (cnv[4] + cnv[5]) + (cnv[6] + cnv[7]) + (cnv[8] + M);
		
		return vec4(sqrt(M/S));
	}
	else
	{
		mat3 I;
		float cnv[2];
		vec3 sample;
		
		/* fetch the 3x3 neighbourhood and use the RGB vector's length as intensity value */
		for (int i=0; i<3; i++)
		for (int j=0; j<3; j++) {
			sample = grayscale(texture(Sampler_Source, tex + vec2((i-1) * TexelSize.x, (j-1) * TexelSize.y))).rgb;
			I[i][j] = length(sample);
		}
		
		/* calculate the convolution values for all the masks */
		for(int i=0;i<2;i++){
			float dp3=dot(G[i][0],I[0])+dot(G[i][1],I[1])+dot(G[i][2],I[2]);
			cnv[i]=dp3*dp3;
		}
		
		return vec4(.5*sqrt(cnv[0]*cnv[0]+cnv[1]*cnv[1]));
	}
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec4 ProcessStatic(vec4 colour, vec2 tex)
{
	vec2 scaledTexSize = TexelScaler * TexelSize;
	vec2 pixel = tex / scaledTexSize;
	vec2 pixInt = vec2(floor(pixel.x), floor(pixel.y));  //Shift forward pixInt to stop origin pixel sticking on single colour
	float rnd = rand(Time * (pixInt + 1.376));
	vec4 result = vec4(rnd, rnd, rnd, 1.0);
	float mask = clamp(colour.a + IgnoreTransparent, 0.0, 1.0);
	return mask * result;
}

vec4 ProcessOldMovieColour(vec4 colour,vec2 tex)
{
	vec2 rndx;
	float dim;
	vec2 sc;
	float scr;
	vec2 rLine;
	vec3 rand;
	rndx=Frame*vec2(.007,.01);
	dim=texture(Sampler_Noise,rndx).r;
	dim*=Dim;
	if(dim>1.){
		dim=1.;
	}
	dim=1.-dim;
	colour.rgb*=dim;
	sc=Frame*vec2(.01,.04);
	sc.x=fract(tex.x+sc.x);
	scr=texture(Sampler_Noise,sc).r;
	scr=2.*scr*(1./Scratch);
	scr=1.-abs(1.-scr);
	scr=max(0.,scr);
	colour.rgb+=scr;
	rLine=(tex+RndLine1)*.33;
	rand=texture(Sampler_Noise,rLine).rgb;
	if(Noise>rand.r){
		colour.rgb=vec3(.1+rand.b*.4);
	}
	
	vec2 dist;
	float flux;
	colour=vec4(.9*colour.r,.8*colour.g,.6*colour.b,colour.a);
	dist=.5-tex;
	flux=RndLine2*.04-.02;
	colour.rgb*=(.4+flux-dot(dist,dist))*2.8;
	colour*=OverExposureColour;
	
	return colour;
}

vec4 ProcessRgbFilter(vec4 current,vec2 tex)
{
	float neg=1.-RgbFilterIntensity;
	vec4 modified=current*neg;
	//We multiple by three as we are roughly filtering each pixel through one of 3 colours so expect about 2/3+ of all colour intensity to be lost across the image
	vec4 filtered=3.*current*RgbFilterIntensity*texture(Sampler_ScanlineMask,vec2(tex.x*NumRgbFiltersHorizontal,tex.y*NumRgbFiltersVertical));
	modified+=filtered;
	return modified;
}

void main()
{
	vec2 tex=ModifyTexCoords(FTex);
	
	vec4 sample=texture(Sampler_Source,tex);
	
	if(DetectEdges == 1)
	{
		sample=mix(sample,ProcessEdgeDetection(tex),EdgeAmount);
	}
	
	if(StaticAmount > 0.0)
	{
		sample=mix(sample,ProcessStatic(sample, tex),StaticAmount);
	}
	
	if(OldMovieAmount > 0.0)
	{
		sample=mix(sample,ProcessOldMovieColour(sample,tex), OldMovieAmount);
	}
	
	if(RgbFilterAmount > 0.0)
	{
		sample=mix(sample,ProcessRgbFilter(sample,tex),RgbFilterAmount);
	}
	
	if(ScanLineAmount > 0.0)
	{
		float ty = 2.0 * TexelSize.y; //Hard coded simple scaline sizing
		sample=mix(sample, sample * mod(tex.y, 2.0 * ty) *(1.0 / ty), ScanLineAmount);
	}
	
	fragColor=sample;
}