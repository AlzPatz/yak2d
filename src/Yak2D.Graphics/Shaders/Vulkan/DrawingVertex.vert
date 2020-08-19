#version 450

layout(set = 0, binding = 0) uniform WorldViewProjection
{
	mat4 WorldMatrix;
    mat4 ScreenMatrix;
};

layout(location = 0) in uint isWorld;
layout(location = 1) in uint TexturingType;

layout(location = 2) in vec3 Position;
layout(location = 3) in vec4 Color;
layout(location = 4) in vec2 TexCoord0;
layout(location = 5) in vec2 TexCoord1;
layout(location = 6) in float TexWeight0;

layout(location = 0) flat out uint FTexturingType;
layout(location = 1) out vec4 FColor;
layout(location = 2) out vec2 FTexCoord0;
layout(location = 3) out vec2 FTexCoord1;
layout(location = 4) out float FTex0Weight;

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

    //was the below in the original open gl shaders?
    gl_Position.z = -gl_Position.z; //Ortho Matrix Appears to Negative up the Z-Component (for OpenGL atleast)

    FColor = Color;
    FTexturingType = TexturingType;

    FTexCoord0 = vec2(TexCoord0.x, TexCoord0.y);
    FTexCoord1 = vec2(TexCoord1.x, 1.0 - TexCoord1.y);

    FTex0Weight = TexWeight0;

    //OpenGL -> Vulkan NDC Space Transform
    gl_Position.y = -gl_Position.y;  
    gl_Position.z = (gl_Position.z + gl_Position.w) / 2.0;
}