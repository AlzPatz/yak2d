#version 330 core

uniform WorldViewProjection
{
	mat4 WorldMatrix;
    mat4 ScreenMatrix;
};

in int isWorld;
in int TexturingType;

in vec3 Position;
in vec4 Color;
in vec2 TexCoord0;
in vec2 TexCoord1;
in float TexWeight0;

flat out int FTexturingType;
out vec4 FColor;
out vec2 FTexCoord0;
out vec2 FTexCoord1;
out float FTex0Weight;

void main(void)
{   
    vec4 pos = vec4(Position.x, Position.y, Position.z, 1.0);

    if(isWorld == 0)
    {
        gl_Position = WorldMatrix * pos;
    }
    else
    {
        gl_Position  = ScreenMatrix * pos;
    }

    gl_Position.z = -gl_Position.z; //Ortho Matrix Appears to Negative up the Z-Component (for OpenGL atleast)

    FColor = Color;
    FTexturingType = TexturingType;

    FTexCoord0 = vec2(TexCoord0.x, 1.0 - TexCoord0.y);
    FTexCoord1 = vec2(TexCoord1.x, 1.0 - TexCoord1.y);
    FTex0Weight = TexWeight0;
}