using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class BloomStageModel : IBloomStageModel
    {
        public void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command) => visitor.DispatchToRenderStage(this, cl, command);
        public void CacheInstanceInVisitor(IRenderStageVisitor visitor) => visitor.CacheStageModel(this);

        public GpuSurface LinearSampledSurface0 { get; private set; }
        public GpuSurface LinearSampledSurface1 { get; private set; }
        public GpuSurface AnistropicallySampledSurface { get; private set; }

        public Vector2 TexelShiftSize { get; private set; }

        public float BrightnessThreshold { get { return _current.BrightnessThreshold; } }
        public float MixAmount { get { return _current.AdditiveMixAmount; } }
        public int NumberSamples { get { return _current.NumberOfBlurSamples; } }
        public ResizeSamplerType SampleType { get { return _current.ReSamplerType; } }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IStartupPropertiesCache _startUpPropertiesCache;
        private readonly ISystemComponents _systemComponents;
        private readonly IGpuSurfaceManager _surfaceManager;
        private readonly IGaussianBlurWeightsAndOffsetsCache _gaussianWeightsAndOffsetsCache;

        private readonly uint _sampleSurfaceWidth;
        private readonly uint _sampleSurfaceHeight;

        private ulong _linearSurfaceId0;
        private ulong _linearSurfaceId1;
        private ulong _anistropicSurfaceId;

        private BloomEffectConfiguration _current;
        private BloomEffectConfiguration _target;
        private BloomEffectConfiguration _previous;
        private bool _isTransitioning;
        private float _transitionTotalTime;
        private float _transitionCurrentTime;
        private float fraction;

        public BloomStageModel(IFrameworkMessenger frameworkMessenger,
                                        IStartupPropertiesCache startUpPropertiesCache,
                                        ISystemComponents systemComponents,
                                        IGpuSurfaceManager surfaceManager,
                                        IGaussianBlurWeightsAndOffsetsCache gaussianWeightsAndOffsetsCache,
                                        uint sampleSurfaceWidth,
                                        uint sampleSurfaceHeight)
        {
            _frameworkMessenger = frameworkMessenger;
            _startUpPropertiesCache = startUpPropertiesCache;
            _systemComponents = systemComponents;
            _surfaceManager = surfaceManager;
            _gaussianWeightsAndOffsetsCache = gaussianWeightsAndOffsetsCache;
            _sampleSurfaceWidth = sampleSurfaceWidth;
            _sampleSurfaceHeight = sampleSurfaceHeight;

            TexelShiftSize = new Vector2(1.0f / (1.0f * _sampleSurfaceWidth), 1.0f / (1.0f * _sampleSurfaceHeight));

            CreateSurfaces();

            _current = new BloomEffectConfiguration()
            {
                BrightnessThreshold = 0.6f,
                AdditiveMixAmount = 0.5f,
                NumberOfBlurSamples = 4
            };

            _isTransitioning = false;
        }

        private void CreateSurfaces()
        {
            _linearSurfaceId0 = CreateSurface(_sampleSurfaceWidth, _sampleSurfaceHeight, SamplerType.Linear);
            _linearSurfaceId1 = CreateSurface(_sampleSurfaceWidth, _sampleSurfaceHeight, SamplerType.Linear);
            _anistropicSurfaceId = CreateSurface(_sampleSurfaceWidth, _sampleSurfaceHeight, SamplerType.Anisotropic);

            LinearSampledSurface0 = _surfaceManager.RetrieveSurface(_linearSurfaceId0);
            LinearSampledSurface1 = _surfaceManager.RetrieveSurface(_linearSurfaceId1);
            AnistropicallySampledSurface = _surfaceManager.RetrieveSurface(_anistropicSurfaceId);
        }

        private ulong CreateSurface(uint width, uint height, SamplerType samplerType)
        {
            return _surfaceManager.CreateRenderSurface(true,
                                                        width,
                                                        height,
                                                        _startUpPropertiesCache.Internal.PixelFormatForRenderingSurfaces,
                                                        false,
                                                        false,
                                                        false,
                                                        samplerType).Id;
        }

        public void SetEffectTransition(ref BloomEffectConfiguration config, ref float transitionSeconds)
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

        public void Update(float seconds)
        {
            if (!_isTransitioning)
            {
                return;
            }

            _transitionCurrentTime += seconds;

            fraction = _transitionCurrentTime / _transitionTotalTime;

            if (fraction >= 1.0f)
            {
                fraction = 1.0f;
                _isTransitioning = false;
            }

            _current.BrightnessThreshold = Utility.Interpolator.Interpolate(_previous.BrightnessThreshold, _target.BrightnessThreshold, ref fraction);
            _current.AdditiveMixAmount = Utility.Interpolator.Interpolate(_previous.AdditiveMixAmount, _target.AdditiveMixAmount, ref fraction);
            _current.NumberOfBlurSamples = Utility.Interpolator.Interpolate(_previous.NumberOfBlurSamples, _target.NumberOfBlurSamples, ref fraction);
            _current.ReSamplerType = fraction == 1.0f ? _target.ReSamplerType : _previous.ReSamplerType;
        }

        public void DestroyResources()
        {
            _surfaceManager.DestroySurface(_linearSurfaceId0);
            _surfaceManager.DestroySurface(_linearSurfaceId1);
            _surfaceManager.DestroySurface(_anistropicSurfaceId);
        }
    }
}