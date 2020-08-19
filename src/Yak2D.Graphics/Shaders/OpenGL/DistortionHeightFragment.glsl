#version 330 core

uniform sampler2D texSampler0;
uniform sampler2D texSampler1;

flat in int FTexturingType;

in vec4 FColor;
in vec2 FTexCoord0;
in vec2 FTexCoord1;
in float FTex0Weight;

out float fragColor;

//Maybe this could work for float 32 as well...? as g naturally 0
void main(void)
{
    //Colour (untextured)
    if(FTexturingType == 0)
    {
        fragColor = FColor.r - FColor.g;
        return;
    }

    if(FTexturingType == 1)
    {
        //Single Texture
        vec4 sampleSingle = FColor * texture(texSampler0, FTexCoord0);  
        fragColor = sampleSingle.r - sampleSingle.g;        
        return;
    }

    //Dual Texture
    vec4 sampleDual = FColor * 
                (
                    (texture(texSampler0, FTexCoord0) * FTex0Weight) +
                    (texture(texSampler1, FTexCoord1) * (1.0 - FTex0Weight))
                );
    fragColor = sampleDual.r - sampleDual.g;
}