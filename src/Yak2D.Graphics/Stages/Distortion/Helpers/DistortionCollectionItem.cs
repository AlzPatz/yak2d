using System.Numerics;

namespace Yak2D.Graphics
{
    public class DistortionCollectionItem
    {
        public bool Active { get; set; }
        public LifeCycle Cycle { get; set; }
        public CoordinateSpace CoordinateSpace { get; set; }
        public ITexture Texture { get; set; }
        public float Duration { get; set; }
        public Vector2 InitSize { get; set; }
        public Vector2 FinalSize { get; set; }
        public Vector2 InitPosition { get; set; }
        public Vector2 FinalPosition { get; set; }
        public float InitIntensity { get; set; }
        public float FinalIntensity { get; set; }
        public float InitRotation { get; set; }
        public float FinalRotation { get; set; }

        public float TimeCount { get; set; }
        public bool CurrentCountDirectionIsForward { get; set; }
    }
}