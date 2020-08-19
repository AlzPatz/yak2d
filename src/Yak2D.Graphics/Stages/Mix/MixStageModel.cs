using System.Numerics;
using Veldrid;

namespace Yak2D.Graphics
{
    public class MixStageModel : IMixStageModel
    {
        public void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command) => visitor.DispatchToRenderStage(this, cl, command);
        public void CacheInstanceInVisitor(IRenderStageVisitor visitor) => visitor.CacheStageModel(this);

        private readonly IFrameworkMessenger _frameworkMessenger;

        public Vector4 MixAmount { get { return _current; } }

        private Vector4 _current;
        private Vector4 _target;
        private Vector4 _previous;
        private bool _isTransitioning;
        private float _transitionTotalTime;
        private float _transitionCurrentTime;
        private float _fraction;

        public MixStageModel(IFrameworkMessenger frameworkMessenger)

        {
            _frameworkMessenger = frameworkMessenger;
            _isTransitioning = false;

        }

        public void SetEffectTransition(ref Vector4 mixAmount, ref float transitionSeconds, bool normalise)
        {
            var amount = new Vector4(mixAmount.X < 0.0f ? 0.0f : mixAmount.X,
                                                            mixAmount.Y < 0.0f ? 0.0f : mixAmount.Y,
                                                            mixAmount.Z < 0.0f ? 0.0f : mixAmount.Z,
                                                            mixAmount.W < 0.0f ? 0.0f : mixAmount.W);

            if (normalise)
            {
                var total = amount.X + amount.Y + amount.Z + amount.W;
                if(total > 1.0f)
                {
                    var scale = 1.0f / total;
                    amount *= scale;
                }
            }

            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue); //Might be duplicated clamp for this value

            if (transitionSeconds == 0.0f)
            {
                _current = amount;
                _isTransitioning = false;
                return;
            }

            _previous = _current;
            _target = amount;
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

            _current = Utility.Interpolator.Interpolate(ref _previous, ref _target, ref _fraction);
        }

        public void DestroyResources()
        {
            //Nothing to Dispose
        }
    }
}