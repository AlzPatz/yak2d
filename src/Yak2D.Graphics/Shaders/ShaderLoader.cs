using System.Linq;
using Veldrid;

namespace Yak2D.Graphics
{
    public class ShaderLoader : IShaderLoader
    {
        private readonly IShaderLoaderFunctions _loadFunctions;

        public ShaderLoader(IShaderLoaderFunctions shaderLoaderFunctions)
        {
            _loadFunctions = shaderLoaderFunctions;
        }

        /*
              Shader File Configuration Notes

              A small number of naming, file extension and entry point restrictions are mandated to enable easy use
              of shaders across each backend.

              Shader Files
              - One file should be used for each Vertex and Fragment Shader
              - Each Vertex shader code file MUST have "Vertex" in the name
              - Each Fragment shader code file MUST have "Fragment" in the name
              - Shaders for each backend sit in their respective sub-folders (Direct3D, Metal, OpenGL and Vulkan)

              - All HLSL shader source files should have extension .hlsl
              - The HLSL compiling script uses the presence of Vertex/Fragment in the source file name to choose the right compiling profile
              - Compiled HLSL code files loaded by the framework have the extension .hlsl.bytes

              - Vulkan Shaders must use the extensions .vert and .frag respectively for Vertex and Fragment shaders
              - Vulkan shader code is compiled to .spv

              - OpenGL Shaders must all use the extension .glsl
              - OpenGL shader code is not compiled, and loaded as text byte strings straight from shader source during runtime

              - Metal shader source files have the extension .metal
              - Metal shaders are compiled to .metallib

              Shader Code
              - The entry point for all Vertex and Pixel Shader functions across backends is main()
              - EXCEPT metal, where main() is not permitted. For metal shaders the function shader() is used

              SPIR-V Additions
              - The main framework avoids relying on SPIR-V compilation from single source due to inconsistencies that can crop up
                either due to user error, SPIR-V issue or backend unique properties. This caution maybe unwarranted, but there was 
                a preference to code each backend shader individually
              - HOWEVER - for custom shaders stages, a SPIR-V .glsl can be used, enabling runtime cross platform compilation
              - The rules for SPIR-V shaders are 1. paths are rooted in Shaders/ base directory, .glsl extension and main() entiry point

              Notes:
              * You may note significant padding in shader uniforms, and the avoidance of some vector types, particularly float/vec3s
              - Most of this is overly defensive and ignores potential reliance that could be had on explicit byte layout guides provided in c# structs
              - The aim here was to avoid the stride differences found in some data types across backends, reducing/removing uniform data problems
              * Compiling HLSL with DXC.exe (the latest compiler) causes issues with SharpDX that are quite opague. Compiling with FXC.exe
              - works for included shaders and is therefore the preferred method. In the future I would like to find out why this issue exists (semantics?)
              * I avoided using Veldrid-SPIRV cross for shader translation across backends to avoid edge case problems,
              - remove another layer of potential debugging, and also to better learn other shader languages
              - With the shader set relatively fixed, it could be ported to single source, cross translation in the future

              Shader Compilation
              * Scripts are provided for compiling all shaders in a directory: .bat file for windows HLSL/Vulkan and .sh for Linux Vulkan and OSX metal
              * .bat files must be given one argument, which is the filepath of the compiler .exe to use
              * .sh shell scripts relay on compiler paths being present on the system
              * NOTE: Currently, the .bat files are able to hand recursive directories of shaders to compile
              * .sh shell scripts currently only compile shaders in same directory as the script (to be modified in the future)

              Compilers 
              * HLSL: Use FXC.exe (I experience issues with DXC.exe)
              * Vulkan: Use glslc from the Vulkan SDK
              * Metal: Use xcrun (will require installation of XCode and associated tools)
        */

        public ShaderPackage CreateShaderPackage(string vertexShaderName,
                                                    AssetSourceEnum vertexShaderAssetType,
                                                    string fragmentShaderName,
                                                    AssetSourceEnum fragmentShaderAssetType,
                                                    VertexLayoutDescription layoutDescription,
                                                    ResourceLayoutElementDescription[][] uniformDescriptions,
                                                    bool useSpirvCompileVertexShader = false,
                                                    bool useSpirvCompileFragmentShader = false)
        {
            if (uniformDescriptions == null)
            {
                throw new Yak2DException("Error loading shader, null uniform resource layout descriptions array provided");
            }

            for (var n = 0; n < uniformDescriptions.Length; n++)
            {
                if (uniformDescriptions[n] == null)
                {
                    throw new Yak2DException("Error loading shader, null uniform resource layout descriptions sub-array provided");
                }
            }

            if (vertexShaderName == null || vertexShaderName.Trim().Any(char.IsWhiteSpace))
            {
                throw new Yak2DException("Error loading shader, Vertex Shader name cannot be null or contain whitespace");
            }
            vertexShaderName = vertexShaderName.Trim();

            if (fragmentShaderName == null || fragmentShaderName.Trim().Any(char.IsWhiteSpace))
            {
                throw new Yak2DException("Error loading shader, Fragment Shader name cannot be null or contain whitespace");
            }
            fragmentShaderName = fragmentShaderName.Trim();

            var uniformResourceLayout = _loadFunctions.CreateUniformResourceLayouts(uniformDescriptions);

            var vertexShader = _loadFunctions.LoadShader(vertexShaderName, vertexShaderAssetType, ShaderStages.Vertex, useSpirvCompileVertexShader);

            if (vertexShader == null)
            {
                throw new Yak2DException(string.Concat("Vertex Shader: ", vertexShaderName, " failed to Load"));
            }

            var fragmentShader = _loadFunctions.LoadShader(fragmentShaderName, fragmentShaderAssetType, ShaderStages.Fragment, useSpirvCompileFragmentShader);

            if (fragmentShader == null)
            {
                throw new Yak2DException(string.Concat("Fragment Shader: ", fragmentShaderName, " failed to Load"));
            }

            return new ShaderPackage
            {
                Description = new ShaderSetDescription(
                    new[]
                    {
                        layoutDescription
                    },
                    new[]
                    {
                        vertexShader,
                        fragmentShader
                    }
                ),
                UniformResourceLayout = uniformResourceLayout
            };
        }
    }
}