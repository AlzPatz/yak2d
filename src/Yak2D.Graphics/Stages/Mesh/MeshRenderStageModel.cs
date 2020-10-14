using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class MeshRenderStageModel : IMeshRenderStageModel
    {
        public void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command) => visitor.DispatchToRenderStage(this, cl, command);
        public void CacheInstanceInVisitor(IRenderStageVisitor visitor) => visitor.CacheStageModel(this);

        private const int NUMBER_OF_LIGHTS = 8; //Must match shader

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IQuadMeshBuilder _quadMeshBuilder;

        public ResourceSet LightPropertiesResource { get; private set; }
        public ResourceSet LightsResource { get; private set; }

        public DeviceBuffer MeshVertexBuffer { get; private set; }
        public uint MeshNumberVertices { get; private set; }

        private DeviceBuffer _lightingPropertiesDeviceBuffer;
        private DeviceBuffer _lightsDeviceBuffer;

        private MeshRenderLightingPropertiesConfiguration _currentProperties;
        private MeshRenderLightingPropertiesConfiguration _targetProperties;
        private MeshRenderLightingPropertiesConfiguration _previousProperties;
        private bool _isTransitioningProperties;
        private float _transitionTotalTimeProperties;
        private float _transitionCurrentTimeProperties;
        private float _fractionProperties;

        private MeshRenderLightConfiguration[] _currentLights;
        private MeshRenderLightConfiguration[] _targetLights;
        private MeshRenderLightConfiguration[] _previousLights;
        private bool _isTransitioningLights;
        private float _transitionTotalTimeLights;
        private float _transitionCurrentTimeLights;
        private float _fractionLights;

        public MeshRenderStageModel(IFrameworkMessenger frameworkMessenger,
                                    ISystemComponents systemComponents,
                                    IQuadMeshBuilder quadMeshBuilder)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _quadMeshBuilder = quadMeshBuilder;

            CreateLights();
            GenerateInitialLightingConfigurations();
            UpdatePropertiesBuffer();
            UpdateLightsBuffer();
            GenerateDefaultMesh();
        }

        private void CreateLights()
        {
            //Lighting Properties - In Fragment Shader
            _lightingPropertiesDeviceBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(LightProperties.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            //Duped layouts from renderer
            LightPropertiesResource = _systemComponents.Factory.CreateResourceSet(
                    new ResourceSetDescription(
                        _systemComponents.Factory.CreateResourceLayout(
                            new ResourceLayoutDescription(
                                   new ResourceLayoutElementDescription("LightProperties", ResourceKind.UniformBuffer, ShaderStages.Fragment)
                                )), _lightingPropertiesDeviceBuffer)
            );

            //Lights - In Fragment Shader
            _lightsDeviceBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(NUMBER_OF_LIGHTS * Light.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            //Duped layouts from renderer
            LightsResource = _systemComponents.Factory.CreateResourceSet(
                    new ResourceSetDescription(
                        _systemComponents.Factory.CreateResourceLayout(
                            new ResourceLayoutDescription(
                                   new ResourceLayoutElementDescription("LightsUniformBlock", ResourceKind.UniformBuffer, ShaderStages.Fragment)
                                )), _lightsDeviceBuffer)
            );
        }

        private void GenerateInitialLightingConfigurations()
        {
            _isTransitioningProperties = false;

            FillInitialProperties();

            _isTransitioningLights = false;
            _currentLights = new MeshRenderLightConfiguration[NUMBER_OF_LIGHTS];
            _targetLights = new MeshRenderLightConfiguration[NUMBER_OF_LIGHTS];
            _previousLights = new MeshRenderLightConfiguration[NUMBER_OF_LIGHTS];

            FillInitialLights();
        }

        private void FillInitialProperties()
        {
            _currentProperties = new MeshRenderLightingPropertiesConfiguration
            {
                SpecularColour = new Vector3(0.5f, 0.5f, 0.5f),
                Shininess = 16.0f,
                NumberOfActiveLights = 1
            };
        }

        private void FillInitialLights()
        {
            _currentLights = new MeshRenderLightConfiguration[NUMBER_OF_LIGHTS];
            for (var n = 0; n < NUMBER_OF_LIGHTS; n++)
            {
                _currentLights[n] = new MeshRenderLightConfiguration
                {
                    LightType = LightType.Directional,
                    Position = new Vector3(0.0f, 0.0f, -1.0f), //3D is Right Handed Coorindate System. This negative Z light points away from camera along negative z
                    Colour = new Vector3(1.0f, 1.0f, 1.0f),
                    ConeDirection = new Vector3(0.0f, 0.0f, 0.0f),
                    Attenuation = 0.0f,
                    AmbientCoefficient = 0.1f,
                    ConeAngle = 0.0f
                };
            }
        }

        public void SetLightingProperties(ref MeshRenderLightingPropertiesConfiguration config, float transitionSeconds)
        {
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            if (transitionSeconds == 0.0f)
            {
                _currentProperties = config;
                _isTransitioningProperties = false;
                UpdatePropertiesBuffer();
                return;
            }

            _previousProperties = _currentProperties;
            _targetProperties = config;
            _transitionTotalTimeProperties = transitionSeconds;
            _transitionCurrentTimeProperties = 0.0f;
            _isTransitioningProperties = true;
        }

        private void UpdatePropertiesBuffer()
        {
            var properties = new LightProperties
            {
                SpecularColour = new Vector4(_currentProperties.SpecularColour, 0.0f),
                Shininess = _currentProperties.Shininess,
                NumLights = _currentProperties.NumberOfActiveLights
            };

            _systemComponents.Device.UpdateBuffer(_lightingPropertiesDeviceBuffer, 0, ref properties);
        }

        public void SetLights(MeshRenderLightConfiguration[] lightConfigurations, float transitionSeconds)
        {
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            if (transitionSeconds == 0.0f)
            {
                _isTransitioningLights = false;

                for (var n = 0; n < NUMBER_OF_LIGHTS; n++)
                {
                    if (n < lightConfigurations.Length)
                    {
                        _currentLights[n] = lightConfigurations[n];
                    }
                    else
                    {
                        n = NUMBER_OF_LIGHTS;
                    }
                }

                UpdateLightsBuffer();
            }

            for (var n = 0; n < NUMBER_OF_LIGHTS; n++)
            {
                if (n < lightConfigurations.Length)
                {
                    _previousLights[n] = _currentLights[n];
                    _targetLights[n] = lightConfigurations[n];
                }
            }
            _transitionTotalTimeLights = transitionSeconds;
            _transitionCurrentTimeLights = 0.0f;
            _isTransitioningLights = true;

            return;
        }

        private void UpdateLightsBuffer()
        {
            var lights = new Light[NUMBER_OF_LIGHTS];

            for (var n = 0; n < NUMBER_OF_LIGHTS; n++)
            {
                var conf = _currentLights[n];

                lights[n] = new Light
                {
                    Position = new Vector4(conf.Position, conf.LightType == LightType.Directional ? 0.0f : 1.0f),
                    Colour = new Vector4(conf.Colour, 1.0f), //a is ignored
                    ConeDirection = new Vector4(conf.ConeDirection, 0.0f), //a is ignored
                    Attenuation = conf.Attenuation,
                    AmbientCoefficient = conf.AmbientCoefficient,
                    ConeAngle = conf.ConeAngle
                };
            }

            _systemComponents.Device.UpdateBuffer(_lightsDeviceBuffer, 0, lights);
        }

        private void GenerateDefaultMesh()
        {
            var width = 240.0f;
            var height = 135.0f;

            SetMesh(_quadMeshBuilder.Build(width, height));
        }

        public void Update(float seconds)
        {
            if (_isTransitioningProperties)
            {
                UpdatePropertiesTransition(seconds);
            }

            if (_isTransitioningLights)
            {
                UpdateLightsTransition(seconds);
            }
        }

        private void UpdatePropertiesTransition(float seconds)
        {
            _transitionCurrentTimeProperties += seconds;

            _fractionProperties = _transitionCurrentTimeProperties / _transitionTotalTimeProperties;

            if (_fractionProperties >= 1.0f)
            {
                _fractionProperties = 1.0f;
                _isTransitioningProperties = false;
            }

            _currentProperties.SpecularColour = Utility.Interpolator.Interpolate( _previousProperties.SpecularColour,  _targetProperties.SpecularColour, ref _fractionProperties);
            _currentProperties.Shininess = Utility.Interpolator.Interpolate( _previousProperties.Shininess,  _targetProperties.Shininess, ref _fractionProperties);
            _currentProperties.NumberOfActiveLights = Utility.Interpolator.Interpolate(_previousProperties.NumberOfActiveLights, _targetProperties.NumberOfActiveLights, ref _fractionProperties);

            UpdatePropertiesBuffer();
        }

        private void UpdateLightsTransition(float seconds)
        {
            _transitionCurrentTimeLights += seconds;

            _fractionLights = _transitionCurrentTimeLights / _transitionTotalTimeLights;

            if (_fractionLights >= 1.0f)
            {
                _fractionLights = 1.0f;
                _isTransitioningLights = false;
            }

            for (var n = 0; n < NUMBER_OF_LIGHTS; n++)
            {
                //Flip type enum at 100%
                _currentLights[n].LightType = _fractionLights == 1.0f ? _targetLights[n].LightType : _previousLights[n].LightType;
                _currentLights[n].Position = Utility.Interpolator.Interpolate(_previousLights[n].Position, _targetLights[n].Position, ref _fractionLights);
                _currentLights[n].Colour = Utility.Interpolator.Interpolate(_previousLights[n].Colour, _targetLights[n].Colour, ref _fractionLights);
                _currentLights[n].ConeDirection = Utility.Interpolator.Interpolate(_previousLights[n].ConeDirection, _targetLights[n].ConeDirection, ref _fractionLights);
                _currentLights[n].Attenuation = Utility.Interpolator.Interpolate(_previousLights[n].Attenuation, _targetLights[n].Attenuation, ref _fractionLights);
                _currentLights[n].AmbientCoefficient = Utility.Interpolator.Interpolate(_previousLights[n].AmbientCoefficient, _targetLights[n].AmbientCoefficient, ref _fractionLights);
                _currentLights[n].ConeAngle = Utility.Interpolator.Interpolate(_previousLights[n].ConeAngle, _targetLights[n].ConeAngle, ref _fractionLights);
            }

            UpdateLightsBuffer();
        }

        public void SetMesh(Vertex3D[] mesh)
        {
            if (mesh == null)
            {
                _frameworkMessenger.Report("Supplied Mesh is null");
                return;
            }

            var numVertices = mesh.Length;

            var remainder = numVertices % 3;

            if (remainder != 0)
            {
                _frameworkMessenger.Report("Supplied mesh number of vertices is not a multiple of 3. Extra vertices being cut... it's all about triangles baby");
                numVertices -= remainder;
            }

            if (numVertices <= 0)
            {
                return;
            }

            MeshNumberVertices = (uint)numVertices;

            MeshVertexBuffer?.Dispose();

            MeshVertexBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(MeshNumberVertices * Vertex3D.SizeInBytes, BufferUsage.VertexBuffer));

            _systemComponents.Device.UpdateBuffer(MeshVertexBuffer, 0, ref mesh[0], MeshNumberVertices * Vertex3D.SizeInBytes);
        }

        public void DestroyResources()
        {
            LightPropertiesResource?.Dispose();
            LightsResource?.Dispose();
            MeshVertexBuffer?.Dispose();
            _lightingPropertiesDeviceBuffer?.Dispose();
            _lightsDeviceBuffer?.Dispose();
        }
    }
}