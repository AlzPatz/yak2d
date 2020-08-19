//Phong Lighting Shader with multiple lights of either Directional, Point or Spot Lights

//static const float gamma =  0.454545454545;

#define MAX_LIGHTS 8

cbuffer FragUniforms: register(b1)
{
	float4 CameraPosition; //Use only 3 components
	float4 Pad0;
};

SamplerState Sampler : register(s0);
Texture2D Texture : register(t0);

cbuffer LightProperties : register(b2)
{
	float4 SpecularColour; //Use only 3 components
	float Shininess;
	int NumLights;
    float2 Pad1;
};

struct Light
{
    float4 Position;
    float4 Colour; //ignore a
    float4 ConeDirection; //ignore w
    float Attenuation;
    float AmbientCoefficient;
    float ConeAngle;
    float Pad;
};

cbuffer LightsUniformBlock : register(b3)
{
	Light Lights[MAX_LIGHTS];
};

struct FragmentIn
{
	float4 Position : SV_Position;
	float3 FragPosition : POSITION;
	float3 FragNormal: NORMAL;
	float2 FragTexCoord : TEXCOORD;
};

float3 CalculateLightContribution(Light light, float3 surfaceColour, float3 surfaceNormal, float3 surfacePosition, float3 surfaceToCamera)
{
    float3 surfaceToLightDirection;

    float attenuation = 1.0;

    if(light.Position.w == 0.0)
    {
        //DIRECTIONAL LIGHT
        surfaceToLightDirection = normalize(-light.Position.xyz);
    }
    else
    {
        //POINT LIGHT
        float3 surfaceToLightDelta = light.Position.xyz - surfacePosition;
        surfaceToLightDirection = normalize(surfaceToLightDelta);
        float distanceToLight = length(surfaceToLightDelta);
        attenuation = 1.0 / (1.0 + (light.Attenuation * pow(distanceToLight, 2)));

        //Cone restrictions (affects attenuation)
        float lightToSurfaceAngle = degrees(acos(dot(-surfaceToLightDirection, normalize(light.ConeDirection.xyz))));
        if(lightToSurfaceAngle > light.ConeAngle){
            attenuation = 0.0;
        }
    }

    //ambient Lighting
    float3 ambient = light.AmbientCoefficient * surfaceColour.rgb * light.Colour.rgb;
   
    //Diffuse Lighting
    float diffuseCoefficient = max(0.0, dot(surfaceNormal, surfaceToLightDirection));
    float3 diffuse = diffuseCoefficient * surfaceColour.rgb * light.Colour.rgb;
    
    //Specular Lighting
    float specularCoefficient = 0.0;
    if (diffuseCoefficient > 0.0 && Shininess > 1.0)
    {
        specularCoefficient = pow(max(0.0, dot(surfaceToCamera, reflect(-surfaceToLightDirection, surfaceNormal))), Shininess);
    }

    float3 specular  = specularCoefficient * SpecularColour.rgb * light.Colour.rgb;
    return ambient + (attenuation * (diffuse + specular));
}

float4 main(FragmentIn fIn) : SV_Target
{
    float4 textureColour = Texture.Sample(Sampler, fIn.FragTexCoord);

    float3 surfaceToCamera = normalize(CameraPosition.xyz - fIn.FragPosition);

    float3 lightingColourSum = float3(0, 0, 0); 

    for(int i = 0; i < NumLights; i++)
    {
        lightingColourSum += CalculateLightContribution(Lights[i], textureColour.rgb, fIn.FragNormal, fIn.FragPosition, surfaceToCamera);
    }

    //Gamma just looks washed out...
    //float3 gam = float3(gamma);
    //return float4(pow(lightingColourSum, gam), textureColour.a);

    return float4(lightingColourSum, 1.0);
}