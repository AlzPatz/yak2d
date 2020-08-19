#version 450

layout(set = 1, binding = 0) uniform texture2D tex0;
layout(set = 1, binding = 1) uniform sampler texSampler0;

layout(set = 2, binding = 0) uniform texture2D tex1;
layout(set = 2, binding = 1) uniform sampler texSampler1;

layout(location = 0) flat in uint FTexturingType;

layout(location = 1) in vec4 FColor;
layout(location = 2) in vec2 FTexCoord0;
layout(location = 3) in vec2 FTexCoord1;
layout(location = 4) in float FTex0Weight;

layout(location = 0) out vec4 fragColor;

void main(void)
{
    if(FTexturingType == 0)
    {
        //Colour Only
        fragColor = FColor;
        return;
    }

    if(FTexturingType == 1)
    {
        //Single Texture
        fragColor = FColor * texture(sampler2D(tex0, texSampler0), FTexCoord0);  
        return;
    }

    //Dual Texture
    fragColor = FColor * 
                (
                    (texture(sampler2D(tex0, texSampler0), FTexCoord0) * FTex0Weight) +
                    (texture(sampler2D(tex1, texSampler1), FTexCoord1) * (1.0 - FTex0Weight))
                );
}