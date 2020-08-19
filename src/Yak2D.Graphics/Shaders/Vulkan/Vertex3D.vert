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
    FragTexCoord = vec2(VertTexCoord.x, 1.0 - VertTexCoord.y);
    FragNormal = VertNormal;
    FragPosition = VertPosition;
    gl_Position = WorldViewProjection * vec4(FragPosition, 1.0);
    
    //gl_Position.z = gl_Position.z * 2.0 - gl_Position.w;

    //OpenGL -> Vulkan NDC Space Transform
    //gl_Position.y = -gl_Position.y; 
    gl_Position.z = (gl_Position.z + gl_Position.w) / 2.0;
}