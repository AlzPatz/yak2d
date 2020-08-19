//Simple Vertex Shader to feed Phong Lighting Model Fragment Shader

//The supplied WVP matrix assumes a right handed coordinate system

#version 330 core

uniform VertexUniforms
{
    mat4 WorldViewProjection;
};

in vec3 VertPosition;
in vec3 VertNormal;
in vec2 VertTexCoord;

out vec3 FragPosition;
out vec3 FragNormal;
out vec2 FragTexCoord;

void main(void)
{
    FragTexCoord = vec2(VertTexCoord.x, 1.0 - VertTexCoord.y);
    FragNormal = VertNormal;
    FragPosition = VertPosition;
    gl_Position = WorldViewProjection * vec4(FragPosition, 1.0);
    gl_Position.z = gl_Position.z * 2.0 - gl_Position.w;
}