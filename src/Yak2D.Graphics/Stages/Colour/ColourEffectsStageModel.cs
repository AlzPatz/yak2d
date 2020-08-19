using System.Numerics;
using Veldrid;
using Yak2D.Internal;
using Yak2D.Utility;

namespace Yak2D.Graphics
{
    public class ColourEffectsStageModel : IColourEffectsStageModel
    {
        public void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command) => visitor.DispatchToRenderStage(this, cl, command);
        public void CacheInstanceInVisitor(IRenderStageVisitor visitor) => visitor.CacheStageModel(this);

        public ResourceSet FactorsResourceSet { get; private set; }
        public RgbaFloat ClearColour { get; private set; }
        public bool ClearBackgroundBeforeRender { get { return _current.ClearBackground; } }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;

        private DeviceBuffer _factorsBuffer;
        private ColourEffectConfiguration _current;
        private ColourEffectConfiguration _target;
        private ColourEffectConfiguration _previous;
        private bool _isTransitioning;
        private float _transitionTotalTime;
        private float _transitionCurrentTime;
        private float _fraction;

        public ColourEffectsStageModel(IFrameworkMessenger frameworkMessenger,
                                        ISystemComponents systemComponents)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;

            _current = new ColourEffectConfiguration()
            {
                ColourForSingleColourAndColourise = new Colour(1.0f, 1.0f, 1.0f, 1.0f),
                BackgroundClearColour = new Colour(0.0f, 0.0f, 0.0f, 0.0f),
                SingleColour = 0.0f,
                Negative = 0.0f,
                Colourise = 0.0f,
                GrayScale = 0.0f,
                Opacity = 1.0f
            };

            _isTransitioning = false;

            CreateBuffer();

            UpdateBufferAndClearColour();
        }

        private void CreateBuffer()
        {
            _factorsBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(ColourEffectFactors.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            //Duplicated across here and renderer...
            var resourceLayoutElementDescription = new ResourceLayoutElementDescription("Factors", ResourceKind.UniformBuffer, ShaderStages.Fragment);

            var resourceLayout = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    resourceLayoutElementDescription
                )
            );

            FactorsResourceSet = _systemComponents.Factory.CreateResourceSet(
                new ResourceSetDescription(resourceLayout, _factorsBuffer)
            );
        }

        private void UpdateBufferAndClearColour()
        {
            ClearColour = ColourConverter.ConvertToRgbaFloat(_current.BackgroundClearColour);

            var factors = new ColourEffectFactors
            {
                SingleColourAmount = _current.SingleColour,
                GrayScaleAmount = _current.GrayScale,
                ColouriseAmount = _current.Colourise,
                NegativeAmount = _current.Negative,
                Colour = ColourConverter.ConvertToVec4(_current.ColourForSingleColourAndColourise),
                Opacity = _current.Opacity,
                Pad0 = 0.0f,
                Pad1 = 0.0f,
                Pad2 = 0.0f,
                Pad3 = Vector4.Zero
            };

            _systemComponents.Device.UpdateBuffer(_factorsBuffer, 0, ref factors);
        }

        public void SetEffectTransition(ref ColourEffectConfiguration config, ref float transitionSeconds)
        {
            transitionSeconds = Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue); //Might be duplicated clamp for this value

            if (transitionSeconds == 0.0f)
            {
                _current = config;
                _isTransitioning = false;
                UpdateBufferAndClearColour();
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

            _fraction = _transitionCurrentTime / _transitionTotalTime;

            if (_fraction >= 1.0f)
            {
                _fraction = 1.0f;
                _isTransitioning = false;
            }

            _current.SingleColour = Interpolator.Interpolate(_previous.SingleColour, _target.SingleColour, ref _fraction);
            _current.Negative = Interpolator.Interpolate(_previous.Negative, _target.Negative, ref _fraction);
            _current.Colourise = Interpolator.Interpolate(_previous.Colourise, _target.Colourise, ref _fraction);
            _current.GrayScale = Interpolator.Interpolate(_previous.GrayScale, _target.GrayScale, ref _fraction);
            _current.Opacity = Interpolator.Interpolate(_previous.Opacity, _target.Opacity, ref _fraction);
            _current.ColourForSingleColourAndColourise = Interpolator.Interpolate(_previous.ColourForSingleColourAndColourise, _target.ColourForSingleColourAndColourise, ref _fraction);
            _current.BackgroundClearColour = Interpolator.Interpolate(_previous.BackgroundClearColour, _target.BackgroundClearColour, ref _fraction);
            _current.ClearBackground = _fraction == 1.0f ? _target.ClearBackground : _previous.ClearBackground;

            UpdateBufferAndClearColour();
        }

        public void DestroyResources()
        {
            _factorsBuffer?.Dispose();
            FactorsResourceSet?.Dispose();
        }
    }
}