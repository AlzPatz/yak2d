using System.IO;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class ShaderLoaderFunctions : IShaderLoaderFunctions
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IAssembly _applicationAssembly;
        private readonly IAssembly _graphicsAssembly;
        private readonly IFileSystem _fileSystem;

        public ShaderLoaderFunctions(IFrameworkMessenger frameworkMessenger,
                                        ISystemComponents systemComponents,
                                        IApplicationAssembly applicationAssembly,
                                        IGraphicsAssembly graphicsAssembly,
                                        IFileSystem fileSystem)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _applicationAssembly = applicationAssembly;
            _graphicsAssembly = graphicsAssembly;
            _fileSystem = fileSystem;
        }

        public ResourceLayout[] CreateUniformResourceLayouts(ResourceLayoutElementDescription[][] elementDescriptions)
        {
            if (elementDescriptions == null)
            {
                return null;
            }

            var numUniforms = elementDescriptions.Length;

            var uniformResourceLayout = new ResourceLayout[numUniforms];

            for (var n = 0; n < numUniforms; n++)
            {
                if (elementDescriptions[n] == null)
                {
                    return null;
                }

                uniformResourceLayout[n] = _systemComponents.Factory.CreateResourceLayout(new ResourceLayoutDescription(elementDescriptions[n]));
            }

            return uniformResourceLayout;
        }

        public Shader LoadShader(string name, AssetSourceEnum assetTypes, ShaderStages stage, bool useSpirvCompile = false)
        {
            //Input path should be '/' folder delimited. It is changed to '.' delimited for embedded resources

            var shaderFileInfo = GetShaderFileInfo(name, _systemComponents.Device, useSpirvCompile);

            var shaderBytes = new byte[] { };
            switch (assetTypes)
            {
                case AssetSourceEnum.File:
                    shaderBytes = TryLoadShaderBytesFromFile(shaderFileInfo);
                    break;
                case AssetSourceEnum.Embedded:
                    shaderFileInfo.Directory = shaderFileInfo.Directory.Replace('/', '.');
                    shaderBytes = TryLoadShaderBytesFromEmbeddedResource(shaderFileInfo);
                    break;
            }

            if (shaderBytes.Length == 0)
            {
                _frameworkMessenger.Report(string.Concat("Unable to load shader file: ",
                                                    shaderFileInfo.Directory, "| ", shaderFileInfo.Name, " | ", shaderFileInfo.Extension,
                                                    ", of type --> ", assetTypes.ToString()));
                return null;
            }

            return useSpirvCompile ? 
                      _systemComponents.Factory.CreateShaderCompileFromSpirv(new ShaderDescription(stage, shaderBytes, shaderFileInfo.EntryPointMethod, true)):
                      _systemComponents.Factory.CreateShader(new ShaderDescription(stage, shaderBytes, shaderFileInfo.EntryPointMethod, true));
        }

        public ShaderFileInfo GetShaderFileInfo(string name, IDevice device, bool spirv)
        {
            string directory;
            string extension;
            string shaderEntryPoint = "main";

            if (spirv)
            {
                extension = "glsl";
                directory = "Shaders";
            }
            else
            {
                switch (device.BackendType)
                {
                    case GraphicsApi.Vulkan:
                        extension = "spv";
                        directory = "Shaders/Vulkan";
                        break;
                    case GraphicsApi.OpenGL:
                        extension = "glsl";
                        directory = "Shaders/OpenGL";
                        break;
                    case GraphicsApi.Direct3D11:
                        extension = "hlsl.bytes";
                        directory = "Shaders/Direct3D";
                        break;
                    case GraphicsApi.Metal:
                        extension = "metallib";
                        directory = "Shaders/Metal";
                        shaderEntryPoint = "shader"; // main() not permitted in metal
                        break;
                    default: throw new System.InvalidOperationException();
                }
            }

            return new ShaderFileInfo
            {
                Name = name,
                Directory = directory,
                Extension = extension,
                EntryPointMethod = shaderEntryPoint
            };
        }

        public byte[] TryLoadShaderBytesFromEmbeddedResource(ShaderFileInfo fileInfo)
        {
            var shaderBytes = new byte[] { };

            var commonPath = string.Concat(fileInfo.Directory, ".", fileInfo.Name, ".", fileInfo.Extension);

            var nameFoundInFrameworkAssembly = IsResourcePathInAssembly(_graphicsAssembly, commonPath);
            var nameFoundInAppAssembly = IsResourcePathInAssembly(_applicationAssembly, commonPath);

            if (!(nameFoundInFrameworkAssembly || nameFoundInAppAssembly))
            {
                _frameworkMessenger.Report("Asset common path not found in framework or application assembly");
                return shaderBytes;
            }

            if (nameFoundInFrameworkAssembly && nameFoundInAppAssembly)
            {
                _frameworkMessenger.Report(string.Concat("Warning: Asset common path found in both framework and application assemblies: using application assembly as override: ", commonPath));
            }

            var assembly = nameFoundInAppAssembly ? _applicationAssembly : _graphicsAssembly;

            var assemblyPath = string.Concat(assembly.Name,
                                             ".",
                                             commonPath);

            var stream = assembly.GetManifestResourceStream(assemblyPath);

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                shaderBytes = memoryStream.ToArray();
            }

            return shaderBytes;
        }

        public bool IsResourcePathInAssembly(IAssembly assembly, string commonPath)
        {
            var assemblyPath = string.Concat(assembly.Name, ".", commonPath);

            var resourceNames = assembly.GetManifestResourceNames();

            return resourceNames.Contains(assemblyPath);
        }

        public byte[] TryLoadShaderBytesFromFile(ShaderFileInfo fileInfo)
        {
            var filePath = Path.Combine(fileInfo.Directory, $"{fileInfo.Name}.{fileInfo.Extension}");

            var shaderBytes = new byte[] { };

            if (_fileSystem.Exists(filePath))
            {
                shaderBytes = _fileSystem.ReadAllBytes(filePath);
            }
            else
            {
                _frameworkMessenger.Report(string.Concat("Error: Unable to find shader file from path: ", filePath));
            }

            return shaderBytes;
        }
    }
}