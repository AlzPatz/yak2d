using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class Blur1DStageModel : IBlur1DStageModel
    {
        public void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command) => visitor.DispatchToRenderStage(this, cl, command);
        public void CacheInstanceInVisitor(IRenderStageVisitor visitor) => visitor.CacheStageModel(this);

        public GpuSurface LinearSampledSurface { get; private set; }
        public GpuSurface AnistropicallySampledSurface { get; private set; }

        public Vector2 TexelShiftSize { get; private set; }
        private Vector2 _unitTexelShiftSize;

        public float MixAmount { get { return _current.MixAmount; } }
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
        private ulong _anistropicSurfaceId;

        private Blur1DEffectConfiguration _current;
        private Blur1DEffectConfiguration _target;
        private Blur1DEffectConfiguration _previous;
        private bool _isTransitioning;
        private float _transitionTotalTime;
        private float _transitionCurrentTime;
        private float fraction;

        public Blur1DStageModel(IFrameworkMessenger frameworkMessenger,
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

            _unitTexelShiftSize = new Vector2(1.0f / (1.0f * _sampleSurfaceWidth), 1.0f / (1.0f * _sampleSurfaceHeight));

            CreateSurfaces();

            _current = new Blur1DEffectConfiguration()
            {
                MixAmount = 0.0f,
                NumberOfBlurSamples = 4,
                BlurDirection = Vector2.UnitX,
                ReSamplerType = ResizeSamplerType.NearestNeighbour
            };

            _isTransitioning = false;

            CalculateTexelShift();
        }

        private void CalculateTexelShift()
        {
            _current.BlurDirection = Vector2.Normalize(_current.BlurDirection);
            TexelShiftSize = _unitTexelShiftSize * _current.BlurDirection;
        }

        private void CreateSurfaces()
        {
            _linearSurfaceId0 = CreateSurface(_sampleSurfaceWidth, _sampleSurfaceHeight, true);
            _anistropicSurfaceId = CreateSurface(_sampleSurfaceWidth, _sampleSurfaceHeight, false);

            LinearSampledSurface = _surfaceManager.RetrieveSurface(_linearSurfaceId0);
            AnistropicallySampledSurface = _surfaceManager.RetrieveSurface(_anistropicSurfaceId);
        }

        private ulong CreateSurface(uint width, uint height, bool linearSampling)
        {
            return _surfaceManager.CreateRenderSurface(true,
                                                        width,
                                                        height,
                                                        _startUpPropertiesCache.Internal.PixelFormatForRenderingSurfaces,
                                                        false,
                                                        false,
                                                        false,
                                                        linearSampling).Id;
        }

        public void SetEffectTransition(ref Blur1DEffectConfiguration config, ref float transitionSeconds)
        {   
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue); //Might be duplicated clamp for this value

            if (transitionSeconds == 0.0f)
            {
                _current = config;
                _isTransitioning = false;
                CalculateTexelShift();
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

            _current.MixAmount = Utility.Interpolator.Interpolate(_previous.MixAmount, _target.MixAmount, ref fraction);
            _current.NumberOfBlurSamples = Utility.Interpolator.Interpolate(_previous.NumberOfBlurSamples, _target.NumberOfBlurSamples, ref fraction);
            _current.BlurDirection = Vector2.Normalize(Utility.Interpolator.Interpolate(_previous.BlurDirection, _target.BlurDirection, ref fraction));
            _current.ReSamplerType = fraction == 1.0f ? _target.ReSamplerType : _previous.ReSamplerType;

            CalculateTexelShift();
        }

        public void DestroyResources()
        {
            _surfaceManager.DestroySurface(_linearSurfaceId0);
            _surfaceManager.DestroySurface(_anistropicSurfaceId);
        }
    }
}