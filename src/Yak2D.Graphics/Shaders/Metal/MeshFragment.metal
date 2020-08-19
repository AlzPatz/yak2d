//Phong Lighting Shader with multiple lights of either Directional, Point or Spot Lights

#include <metal_stdlib>
using namespace metal;

//constant float gamma = 0.454545454545;

#define MAX_LIGHTS 8

struct FragUniforms
{
	float4 CameraPosition; //Use only 3 components
    float4 Pad0;
};

struct LightProperties
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

struct LightsUniformBlock
{
	Light Lights[MAX_LIGHTS];
};

struct FragmentIn
{
    float4 Position [[attribute(0)]];
    float3 FragPosition [[attribute(1)]];
    float3 FragNormal [[attribute(2)]];
    float2 FragTexCoord [[attribute(3)]];
};

float3 CalculateLightContribution(Light light,constant LightProperties &lightProperties, float3 surfaceColour, float3 surfaceNormal, float3 surfacePosition, float3 surfaceToCamera)
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
        float lightToSurfaceAngle = acos(dot(-surfaceToLightDirection, normalize(light.ConeDirection.xyz)));
        lightToSurfaceAngle = 180.0 * lightToSurfaceAngle / 3.14159265359;
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
    if(diffuseCoefficient > 0.0 && lightProperties.Shininess > 1.0)
    {
        specularCoefficient = pow(max(0.0, dot(surfaceToCamera, reflect(-surfaceToLightDirection, surfaceNormal))), lightProperties.Shininess);
    }
    float3 specular  = specularCoefficient * lightProperties.SpecularColour.rgb * light.Colour.rgb;
    return ambient + (attenuation * (diffuse + specular));
}

fragment float4 shader( FragmentIn fIn [[stage_in]],
                    texture2d<float, access::sample> Texture [[ texture(0)]],
                    sampler Sampler [[ sampler(0)]],
                    constant FragUniforms &fragUniforms [[buffer(1)]],              //Buffers for uniforms start at slot 1 as the Vertex shader fills 0
                    constant LightProperties &lightProperties [[buffer(2)]],
                    constant LightsUniformBlock &lightUniformBlock [[buffer(3)]])
                    {
                        float4 textureColour = Texture.sample(Sampler, fIn.FragTexCoord);

                        float3 surfaceToCamera = normalize(fragUniforms.CameraPosition.xyz - fIn.FragPosition);

                        float3 lightingColourSum = float3(0, 0, 0); 

                        for(int i = 0; i < lightProperties.NumLights; i++)
                        {
                            lightingColourSum += CalculateLightContribution(lightUniformBlock.Lights[i], lightProperties, textureColour.rgb, fIn.FragNormal, fIn.FragPosition, surfaceToCamera);
                        }

                        //Gamma just looks washed out...
                        //float3 gam = float3(gamma);
                        //return float4(pow(lightingColourSum, gam), textureColour.a);

                        return float4(lightingColourSum, 1.0);
                    }