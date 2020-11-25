using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DistortionStageModel : DrawStageModel, IDistortionStageModel
    {
        public new void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command) => visitor.DispatchToRenderStage(this, cl, command);
        public new void CacheInstanceInVisitor(IRenderStageVisitor visitor) => visitor.CacheStageModel(this);

        public GpuSurface HeightMapSurface { get; private set; }
        public GpuSurface GradientShiftSurface { get; private set; }
        public ResourceSet InternalSurfacePixelShiftUniform { get; private set; }
        public float DistortionScalar { get { return _current.DistortionScalar; } }

        private readonly ISystemComponents _systemComponents;
        private readonly IGpuSurfaceManager _surfaceManager;

        private ulong _heightMapSurfaceId;
        private ulong _gradientShiftSurfaceId;
        private DeviceBuffer _pixelSizeFactorBuffer;

        private DistortionEffectConfiguration _current;
        private DistortionEffectConfiguration _previous;
        private DistortionEffectConfiguration _target;
        private bool _isTransitioning;
        private float _transitionTotalTime;
        private float _transitionCurrentTime;

        public DistortionStageModel(  IDrawQueueGroup queues,
                                            IDrawStageBatcher batcher,
                                            ISystemComponents systemComponents,
                                            IGpuSurfaceManager surfaceManager,
                                            uint internalSurfaceWidth,
                                            uint internalSurfaceHeight)
                                            : base(batcher, queues, BlendState.Alpha)
        {
            _systemComponents = systemComponents;
            _surfaceManager = surfaceManager;

            CreateAndSetPixelSizeUniform(internalSurfaceWidth, internalSurfaceHeight);
            CreateSurfaces(internalSurfaceWidth, internalSurfaceHeight);

            _current = new DistortionEffectConfiguration
            {
                DistortionScalar = 20.0f //Should scale in relation to window height to keep effect consistent. is turned into a vector using aspect ratio in renderer
            };
        }

        private void CreateAndSetPixelSizeUniform(uint width, uint height)
        {
            //Duped from renderer
            _pixelSizeFactorBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(PixelSizeUniform.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var pixelSizeFactorBufferResourceLayout = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("PixelSizeFactor", ResourceKind.UniformBuffer, ShaderStages.Fragment)
                )
            );

            InternalSurfacePixelShiftUniform = _systemComponents.Factory.CreateResourceSet(
                    new ResourceSetDescription(pixelSizeFactorBufferResourceLayout, _pixelSizeFactorBuffer)
            );

            var factor = new PixelSizeUniform
            {
                PixelShift = new Vector2(1.0f / width, 1.0f / height),
                Pad0 = Vector2.Zero,
                Pad1 = Vector4.Zero
            };

            _systemComponents.Device.UpdateBuffer(_pixelSizeFactorBuffer, 0, ref factor);
        }

        private void CreateSurfaces(uint width, uint height)
        {
            _heightMapSurfaceId = CreateSurface(width, height, SamplerType.Linear, PixelFormat.R32_Float);
            _gradientShiftSurfaceId = CreateSurface(width, height, SamplerType.Linear, PixelFormat.R32_G32_Float);

            HeightMapSurface = _surfaceManager.RetrieveSurface(_heightMapSurfaceId);
            GradientShiftSurface = _surfaceManager.RetrieveSurface(_gradientShiftSurfaceId);
        }

        private ulong CreateSurface(uint width, uint height, SamplerType samplerType, PixelFormat pixelFormat)
        {
            return _surfaceManager.CreateRenderSurface(true,
                                                        width,
                                                        height,
                                                        pixelFormat,
                                                        false,
                                                        false,
                                                        false,
                                                        samplerType,
                                                        1,
                                                        TexSampleCount.X1).Id;
        }

        public void SetEffectTransition(ref DistortionEffectConfiguration config, ref float transitionSeconds)
        {
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue); //Might be duplicated clamp for this value

            if (transitionSeconds == 0.0f)
            {
                _current = config;
                _isTransitioning = false;
                return;
            }

            _previous = _current;
            _target = config;
            _transitionTotalTime = transitionSeconds;
            _transitionCurrentTime = 0.0f;
            _isTransitioning = true;
        }

        public override void Update(float seconds)
        {
            if (!_isTransitioning)
            {
                return;
            }

            _transitionCurrentTime += seconds;

            var fraction = _transitionCurrentTime / _transitionTotalTime;

            if (fraction >= 1.0f)
            {
                fraction = 1.0f;
                _isTransitioning = false;
            }

            _current.DistortionScalar = Utility.Interpolator.Interpolate(_previous.DistortionScalar, _target.DistortionScalar, ref fraction);
        }
    }
}