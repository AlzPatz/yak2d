//Simple Vertex Shader to feed Phong Lighting Model Fragment Shader

//The supplied WVP matrix assumes a right handed coordinate system

#version 450

layout(set = 0, binding = 0) uniform VertexUniforms
{
    mat4 WorldViewProjection;
};

layout(location = 0) in vec3 VertPosition;
layout(location = 1) in vec3 VertNormal;
layout(location = 2) in vec2 VertTexCoord;

layout(location = 0) out vec3 FragPosition;
layout(location = 1) out vec3 FragNormal;
layout(location = 2) out vec2 FragTexCoord;

void main(void)
{
    FragTexCoord = VertTexCoord;
    FragNormal = VertNormal;
    FragPosition = VertPosition;
    gl_Position = WorldViewProjection * vec4(FragPosition, 1.0);
    //Reconcile the difference between D3D11 and Vulkan clip space coordinates:
    gl_Position.y = -gl_Position.y; 
}