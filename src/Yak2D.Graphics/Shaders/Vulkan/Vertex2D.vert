#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec2 VTex;

layout(location = 0) out vec2 FTex;

void main(void)
{
    FTex = vec2(VTex.x, 1.0 - VTex.y);
    gl_Position = vec4(Position.x, Position.y, 0.5, 1.0);

    //OpenGL -> Vulkan NDC Space Transform
    //gl_Position.z = (gl_Position.z + gl_Position.w) / 2.0; //Not sure if removing this is a good idea
}