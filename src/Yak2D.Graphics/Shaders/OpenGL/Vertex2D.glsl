#version 330 core

in vec3 Position;
in vec2 VTex;

out vec2 FTex;

void main(void)
{
    FTex = vec2(VTex.x, 1.0 - VTex.y);
    gl_Position = vec4(Position.x, Position.y, Position.z, 1.0f);
}