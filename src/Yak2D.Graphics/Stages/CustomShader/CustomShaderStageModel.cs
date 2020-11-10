using System.Collections.Generic;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class CustomShaderStageModel : ICustomShaderStageModel
    {
        //Public accessor properties and methods used by Renderer
        public Pipeline Pipeline { get; private set; }
        public int NumberUserUniforms => _userUniformDescriptions.Length;
        public ShaderUniformType UserUniformType(int index)
        {
            if (InRange(index))
            {
                return _userUniformDescriptions[index].UniformType;
            }

            _frameworkMessenger.Report("Shader uniform Type requested outside of uniform array, returning data");
            return ShaderUniformType.Data;
        }

        public string UserUniformName(int index)
        {
            if (InRange(index))
            {
                return _userUniformDescriptions[index].Name;
            }

            _frameworkMessenger.Report("Shader uniform Name requested outside of uniform array, returning empty string");
            return "";
        }

        public ResourceSet UserUniformResourceSet(int index)
        {
            if (InRange(index))
            {
                return _uniformBuffers[index].ResouceSet;
            }

            _frameworkMessenger.Report("Shader uniform ResourceSet requested outside of uniform array, returning null");
            return null;
        }

        //Name look up included to aid user value setting via uniform name
        public ResourceSet UserUniformResourceSet(string name)
        {
            if (_uniformBufferNameLookup.ContainsKey(name))
            {
                return _uniformBuffers[_uniformBufferNameLookup[name]].ResouceSet;
            }

            //Part of user control path, throwing exception
            throw new Yak2DException(string.Concat("Invalid Uniform Index requested: ", name, " from Custom Shader"));
        }

        private bool InRange(int index)
        {
            return index >= 0 && index < _userUniformDescriptions.Length;
        }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IBlendStateConverter _blendStateConverter;
        private readonly string _fragmentShaderFilename;
        private readonly AssetSourceEnum _shaderFileAssetType;
        private readonly ShaderUniformDescription[] _userUniformDescriptions;
        private readonly BlendState _blendState;
        private readonly bool _useSpirvCompile;

        private UniformBuffer[] _uniformBuffers;
        private Dictionary<string, int> _uniformBufferNameLookup;
        private ShaderPackage _shaderPackage;

        public void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command) => visitor.DispatchToRenderStage(this, cl, command);
        public void CacheInstanceInVisitor(IRenderStageVisitor visitor) => visitor.CacheStageModel(this);

        public CustomShaderStageModel(IFrameworkMessenger frameworkMessenger,
                                 ISystemComponents systemComponents,
                                 IShaderLoader shaderLoader,
                                 IPipelineFactory pipelineFactory,
                                 IBlendStateConverter blendStateConverter,
                                 string fragmentShaderFilename,
                                 AssetSourceEnum shaderFileAssetType,
                                 ShaderUniformDescription[] userUniformDescriptions,
                                 BlendState blendState, 
                                 bool userSpirvCompile)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _blendStateConverter = blendStateConverter;
            _fragmentShaderFilename = fragmentShaderFilename;
            _shaderFileAssetType = shaderFileAssetType;
            _userUniformDescriptions = userUniformDescriptions;
            _blendState = blendState;
            _useSpirvCompile = userSpirvCompile;

            Initialise();
        }

        private void Initialise()
        {
            _uniformBuffers = new UniformBuffer[_userUniformDescriptions.Length];
            _uniformBufferNameLookup = new Dictionary<string, int>();

            CreateShaders();
            CreatePipeline();
        }

        private void CreateShaders()
        {
            //Vertex layout is the same for all custom shaders
            var vertexLayout = new VertexLayoutDescription
            (
                16,
                0,
                new VertexElementDescription[]
                {
                    new VertexElementDescription("Position", VertexElementFormat.Float2, VertexElementSemantic.Position),
                    new VertexElementDescription("VTex", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate)
                }
            );

            //Build up the uniform descriptions & create buffers where required
            var uniformDescriptions = new List<ResourceLayoutElementDescription[]>();

            var numUniforms = _userUniformDescriptions.Length;

            var textureCount = 0;
            for (var n = 0; n < numUniforms; n++)
            {
                var desc = _userUniformDescriptions[n];

                switch (desc.UniformType)
                {
                    case ShaderUniformType.Texture:
                        if (textureCount >= 4)
                        {
                            _frameworkMessenger.Report("Customer Shader has more than 4 textures, not currently allowed, skipping uniform");
                            continue;
                        }
                        textureCount++;

                        uniformDescriptions.Add(
                            new ResourceLayoutElementDescription[]
                            {
                                new ResourceLayoutElementDescription(string.Concat("Sampler_", desc.Name), ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                                new ResourceLayoutElementDescription(string.Concat("Texture_", desc.Name), ResourceKind.Sampler, ShaderStages.Fragment)
                            }
                        );
                        break;
                    case ShaderUniformType.Data:
                        var description = new ResourceLayoutElementDescription[]
                        {
                            new ResourceLayoutElementDescription(desc.Name, ResourceKind.UniformBuffer, ShaderStages.Fragment)
                        };
                        uniformDescriptions.Add(description);
                        var buffer = CreateUniformBuffer(description, desc);
                        _uniformBuffers[n] = buffer;
                        _uniformBufferNameLookup.Add(desc.Name, n);
                        break;
                }
            }

            var vertexShaderFileName = "Vertex2D";

            if (_useSpirvCompile)
            {
                //Different Vertex Shaders account for differences in backend (incl. Texcoord origin)
                switch (_systemComponents.Device.BackendType)
                {
                    case GraphicsApi.Direct3D11:
                    case GraphicsApi.Metal:
                        vertexShaderFileName = "Vertex2D-TcTopLeft";
                        break;
                    case GraphicsApi.Vulkan:
                    case GraphicsApi.OpenGL:
                        vertexShaderFileName = "Vertex2D-TcBottomLeft";
                        break;
                    case GraphicsApi.OpenGLES:
                    case GraphicsApi.SystemDefault:
                        throw new Yak2DException("Type is either unsupported or erroneously stored as system default");
                }
            }

            _shaderPackage = _shaderLoader.CreateShaderPackage(vertexShaderFileName,
                                                               AssetSourceEnum.Embedded,
                                                               _fragmentShaderFilename,
                                                               _shaderFileAssetType,
                                                               vertexLayout,
                                                               uniformDescriptions.ToArray(),
                                                               _useSpirvCompile,
                                                               _useSpirvCompile);
        }

        private UniformBuffer CreateUniformBuffer(ResourceLayoutElementDescription[] description, ShaderUniformDescription desc)
        {
            var buffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(desc.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var resourceLayout = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    description
                )
            );

            var resourceSet = _systemComponents.Factory.CreateResourceSet(
                new ResourceSetDescription(resourceLayout, buffer)
            );

            return new UniformBuffer
            {
                Name = desc.Name,
                Buffer = buffer,
                ResouceSet = resourceSet
            };
        }

        private void CreatePipeline()
        {
            Pipeline = _pipelineFactory.CreateAPipeline(_shaderPackage.UniformResourceLayout,
                                                        _shaderPackage.Description,
                                                        _systemComponents.Device.SwapchainFramebufferOutputDescription,
                                                        _blendStateConverter.Convert(_blendState),
                                                        false);
        }

        public void SetUniformValue<T>(string uniformName, T data) where T : struct
        {
            if (_uniformBufferNameLookup.ContainsKey(uniformName))
            {
                var buffer = _uniformBuffers[_uniformBufferNameLookup[uniformName]].Buffer;
                _systemComponents.Device.UpdateBuffer(buffer, 0, ref data);
            }
            else
            {
                _frameworkMessenger.Report("Error: invalid uniform string name when trying to update custom shader uniform data");
            }
        }

        public void SetUniformValue<T>(string uniformName, T[] dataArray) where T : struct
        {
            if (_uniformBufferNameLookup.ContainsKey(uniformName))
            {
                var buffer = _uniformBuffers[_uniformBufferNameLookup[uniformName]].Buffer;
                _systemComponents.Device.UpdateBuffer(buffer, 0, dataArray);
            }
            else
            {
                _frameworkMessenger.Report("Error: invalid uniform string name when trying to update custom shader uniform data");
            }
        }

        public void DestroyResources()
        {
            foreach (var uniform in _uniformBuffers)
            {
                uniform?.Buffer?.Dispose();
                uniform?.ResouceSet?.Dispose();
            }
            Pipeline?.Dispose();
        }

        public void Update(float seconds) { }
    }
}