//Phong Lighting Shader with multiple lights of either Directional, Point or Spot Lights

#version 450

//const float gamma = 0.454545454545;

layout(set = 1, binding = 0) uniform FragUniforms
{
    uniform vec4 CameraPosition; //Use only 3 components
	uniform vec4 Pad0;
};

layout(set = 2, binding = 0) uniform texture2D Texture; 
layout(set = 2, binding = 1) uniform sampler Sampler; 

#define MAX_LIGHTS 8
layout(set = 3, binding = 0) uniform LightProperties
{
    vec4 SpecularColour; //Use only 3 components
    float Shininess;
    int NumLights;
    vec2 Pad1;
};

struct Light
{
    vec4 Position;
    vec4 Colour; //ignore a
    vec4 ConeDirection; //ignore w
    float Attenuation;
    float AmbientCoefficient;
    float ConeAngle;
    float Pad;
};

layout(set = 4, binding = 0) uniform LightsUniformBlock
{
    Light[MAX_LIGHTS] Lights;
};

layout(location = 0) in vec3 FragPosition;
layout(location = 1) in vec3 FragNormal;
layout(location = 2) in vec2 FragTexCoord;

layout(location = 0) out vec4 fragColor;

vec3 CalculateLightContribution(Light light, vec3 surfaceColour, vec3 surfaceNormal, vec3 surfacePosition, vec3 surfaceToCamera)
{
    light.Position.y = -light.Position.y; // Fixes light position issue versus the other APIs

    vec3 surfaceToLightDirection;

    float attenuation = 1.0;

    if(light.Position.w == 0.0)
    {
        //DIRECTIONAL LIGHT
        surfaceToLightDirection = normalize(-light.Position.xyz);
    }
    else
    {
        //POINT LIGHT
        vec3 surfaceToLightDelta = light.Position.xyz - surfacePosition;
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
    vec3 ambient = light.AmbientCoefficient * surfaceColour.rgb * light.Colour.rgb;
   
    //Diffuse Lighting
    float diffuseCoefficient = max(0.0, dot(surfaceNormal, surfaceToLightDirection));
    vec3 diffuse = diffuseCoefficient * surfaceColour.rgb * light.Colour.rgb;
    
    //Specular Lighting
    float specularCoefficient = 0.0;
    if(diffuseCoefficient > 0.0 && Shininess > 1.0)
    {
        specularCoefficient = pow(max(0.0, dot(surfaceToCamera, reflect(-surfaceToLightDirection, surfaceNormal))), Shininess);
    }
    vec3 specular  = specularCoefficient * SpecularColour.rgb * light.Colour.rgb;
    return ambient + (attenuation * (diffuse + specular));
}

void main(void) 
{
    vec2 tex = FragTexCoord;

    vec4 textureColour = texture(sampler2D(Texture, Sampler), tex);

    vec3 surfaceToCamera = normalize(CameraPosition.xyz - FragPosition);

    vec3 lightingColourSum = vec3(0); 

    for(int i = 0; i < NumLights; i++)
    {
        lightingColourSum += CalculateLightContribution(Lights[i], textureColour.rgb, FragNormal, FragPosition, surfaceToCamera);
    }

    //Gamma just looks washed out...
    //vec3 gam = vec3(gamma);
    //fragColor = vec4(pow(lightingColourSum, gam), textureColour.a);

    fragColor = vec4(lightingColourSum, 1.0);
}