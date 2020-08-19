#version 330 core

uniform sampler2D texSampler0;
uniform sampler2D texSampler1;

flat in int FTexturingType;

in vec4 FColor;
in vec2 FTexCoord0;
in vec2 FTexCoord1;
in float FTex0Weight;

out vec4 fragColor;

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
        fragColor = FColor * texture(texSampler0, FTexCoord0);  
        return;
    }

    //Dual Texture
    fragColor = FColor * 
                (
                    (texture(texSampler0, FTexCoord0) * FTex0Weight) +
                    (texture(texSampler1, FTexCoord1) * (1.0 - FTex0Weight))
                );
}