#version 450

layout(set = 0, binding = 0) uniform texture2D Texture_Source; 
layout(set = 0, binding = 1) uniform sampler Sampler_Source; 

layout(set = 1, binding = 0) uniform texture2D Texture_Noise; 
layout(set = 1, binding = 1) uniform sampler Sampler_Noise; 

layout(set = 2, binding = 0) uniform texture2D Texture_ScanlineMask; 
layout(set = 2, binding = 1) uniform sampler Sampler_ScanlineMask; 

layout(set = 3, binding = 0) uniform PixellateFactors
{
	float PixAmount;
	int NumXDivisions;
	int NumYDivisions;
	float Pad0;
	vec2 TexelSize; //This is a shared property. It hi-jacks this set as Vulkan hit limits at 8 layouts
	vec2 Pad1;
};

layout(set = 4, binding = 0) uniform EdgeDetectionFactors
{
	int DetectEdges;
	int IsFreichen;
	float EdgeAmount;
	vec4 Pad2; 
};

layout(set = 5, binding = 0) uniform StaticFactors
{
	float StaticAmount;
	float Time;
	int IgnoreTransparent;
	float TexelScaler;
	vec4 Pad3;
};

layout(set = 6, binding = 0) uniform OldMovieFactors
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

layout(set = 7, binding = 0) uniform CrtEffectFactors
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

const mat3 G[2]=mat3[](
	mat3(1.,2.,1.,0.,0.,0.,-1.,-2.,-1.),
	mat3(1.,0.,-1.,2.,0.,-2.,1.,0.,-1.)
);

const mat3 H[9]=mat3[](
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

layout(location = 0) in vec2 FTex;
layout(location = 0) out vec4 fragColor;

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
		shift=texture(sampler2D(Texture_Noise, Sampler_Noise),mrnd).r;
		if(shift>RndShiftCutOff)
		{
			yshift=(-1.+(2.*shift))*RndShiftScalar;
		}
		//movieTex.y-=yshift;
		movieTex.y+=yshift; //Ensure roll up
		modified=mix(modified,movieTex,OldMovieAmount);
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
		vec3 csample;
		
		/* fetch the 3x3 neighbourhood and use the RGB vector's length as intensity value */
		for (int i=0; i<3; i++)
		for (int j=0; j<3; j++) {
			csample = grayscale(texture(sampler2D(Texture_Source, Sampler_Source), tex + vec2((i-1) * TexelSize.x, (j-1) * TexelSize.y))).rgb;
			I[i][j] = length(csample);
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
		vec3 csample;
		
		/* fetch the 3x3 neighbourhood and use the RGB vector's length as intensity value */
		for (int i=0; i<3; i++)
		for (int j=0; j<3; j++) {
			csample = grayscale(texture(sampler2D(Texture_Source, Sampler_Source), tex + vec2((i-1) * TexelSize.x, (j-1) * TexelSize.y))).rgb;
			I[i][j] = length(csample);
		}
		
		/* calculate the convolution values for all the masks */
		for(int i=0;i<2;i++){
			float dp3=dot(G[i][0],I[0])+dot(G[i][1],I[1])+dot(G[i][2],I[2]);
			cnv[i]=dp3*dp3;
		}
		
		return vec4(.5*sqrt(cnv[0]*cnv[0]+cnv[1]*cnv[1]));
	}
}

// Gold Noise ©2015 dcerisano@standard3d.com 
//  - based on the Golden Ratio, PI and Square Root of Two
//  - superior distribution
//  - fastest noise generator function
//  - works with all chipsets (including low precision)
float PHI = 1.61803398874989484820459 * 00000.1; // Golden Ratio   
float PI  = 3.14159265358979323846264 * 00000.1; // PI
float SQ2 = 1.41421356237309504880169 * 10000.0; // Square Root of Two
float gold_noise(in vec2 coordinate, in float seed){
    return fract(tan(distance(coordinate*(seed+PHI), vec2(PHI, PI)))*SQ2);
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec4 ProcessStatic(vec4 colour, vec2 tex)
{
	vec2 scaledTexSize = TexelScaler * TexelSize;
	vec2 pixel = tex / scaledTexSize;
	vec2 pixInt = vec2(floor(pixel.x), floor(pixel.y));

	float rnd = rand(Time * pixInt);
	//float rnd = gold_noise(pixInt, Time); //Not sure if allowed to use Gold Noise as has silly (c) symbol...
	//float rnd = rand(texture(Sampler_Noise, Time * pixInt).xy); // Don't really need the extra random csample...
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
	dim=texture(sampler2D(Texture_Noise, Sampler_Noise),rndx).r;
	dim*=Dim;
	if(dim>1.){
		dim=1.;
	}
	dim=1.-dim;
	colour.rgb*=dim;
	sc=Frame*vec2(.01,.04);
	sc.x=fract(tex.x+sc.x);
	scr=texture(sampler2D(Texture_Noise, Sampler_Noise),sc).r;
	scr=2.*scr*(1./Scratch);
	scr=1.-abs(1.-scr);
	scr=max(0.,scr);
	colour.rgb+=scr;
	rLine=(tex+RndLine1)*.33;
	rand=texture(sampler2D(Texture_Noise, Sampler_Noise),rLine).rgb;
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
	vec4 filtered=3.*current*RgbFilterIntensity*texture(sampler2D(Texture_ScanlineMask, Sampler_ScanlineMask),vec2(tex.x*NumRgbFiltersHorizontal,tex.y*NumRgbFiltersVertical));
	modified+=filtered;
	return modified;
}

void main()
{
	vec2 tex=ModifyTexCoords(FTex);
	
	vec4 csample=texture(sampler2D(Texture_Source, Sampler_Source),tex);
	
	if(DetectEdges == 1)
	{
		csample=mix(csample,ProcessEdgeDetection(tex),EdgeAmount);
	}
	
	if(StaticAmount > 0.0)
	{
		csample=mix(csample,ProcessStatic(csample, tex),StaticAmount);
	}
	
	if(OldMovieAmount > 0.0)
	{
		csample=mix(csample,ProcessOldMovieColour(csample,tex),OldMovieAmount);
	}
	
	if(RgbFilterAmount > 0.0)
	{
		csample=mix(csample,ProcessRgbFilter(csample,tex),RgbFilterAmount);
	}
	
	if(ScanLineAmount > 0.0)
	{
		csample=mix(csample,csample*mod(tex.y,TexelSize.y*2.)*(1./TexelSize.y),ScanLineAmount);
	}
	
	fragColor=csample;
}