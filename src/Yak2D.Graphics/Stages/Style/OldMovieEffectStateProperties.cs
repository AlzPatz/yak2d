using System.Numerics;

namespace Yak2D.Graphics
{
    public struct OldMovieEffectStateProperties
    {
        public float Frame;
        public float RndLine1;
        public float RndLine2;
        public bool IsRolling;
        public float CpuShift;
        public float RollSpeed;
        public float RollAcceleration;
        public float RollPosition;

        public bool IsOverExposureFlickering;
        public int OverExposureOscillations;
        public Vector4 OverExposureColour;
        public float OverExposureFlickerTime;
        public float OverExposureFlickerCount;
        public float OverExposureFlickerFrac;
        public float OverExposureAngle;
        public float OverExposureOpacity;
        public float OverExposureIntensity;
        public float OverExposureTotalAngle;
    }
}