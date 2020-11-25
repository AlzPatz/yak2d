using SixLabors.ImageSharp.PixelFormats;
using System;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DistortionTextureGenerator : IDistortionTextureGenerator
    {
        private IGpuSurfaceManager _gpuSurfaceManager;

        public DistortionTextureGenerator(IGpuSurfaceManager gpuSurfaceManager)
        {
            _gpuSurfaceManager = gpuSurfaceManager;
        }

        public ITexture ConcentricSinusoidalFloat32(uint textureWidth, uint textureHeight, int numQuarterWaves, bool taperInner, bool taperOuter, SineWavePoint centreWaveStartPoint = SineWavePoint.ZeroAscending, float maxAmplitude = 1)
        {
            var data = ConcentricSinusoidal(textureWidth, textureHeight, numQuarterWaves, taperInner, taperOuter, centreWaveStartPoint, maxAmplitude);
            return _gpuSurfaceManager.CreateFloat32TextureFromPixelData(textureWidth, textureHeight, data, SamplerType.Anisotropic);
        }

        public ITexture ConcentricSinusoidalRgba(uint textureWidth, uint textureHeight, int numQuarterWaves, bool taperInner, bool taperOuter, SineWavePoint centreWaveStartPoint = SineWavePoint.ZeroAscending, float maxAmplitude = 1)
        {
            var data = ConcentricSinusoidal(textureWidth, textureHeight, numQuarterWaves, taperInner, taperOuter, centreWaveStartPoint, maxAmplitude);
            var pixelcount = textureWidth * textureHeight;
            var rgba = new Rgba32[pixelcount];
            for (var p = 0; p < pixelcount; p++)
            {
                var value = data[p];
                rgba[p] = new Rgba32(
                            value > 0.0f ? value : 0.0f,
                            value < 0.0f ? -value : 0.0f,
                            0.0f, 0.2f //Alpha 1 so we can review it in normal draw pipeline
                        );
            }
            return _gpuSurfaceManager.CreateRgbaTextureFromPixelData(textureWidth, textureHeight, rgba, SamplerType.Anisotropic, true, false);
        }

        private float[] ConcentricSinusoidal(uint textureWidth, uint textureHeight, int numHalfWaves, bool taperInner, bool taperOuter, SineWavePoint centreWaveStartPoint, float maxAmplitude)
        {
            var data = new float[textureWidth * textureHeight];

            var hw = 0.5f * textureWidth;
            var hh = 0.5f * textureHeight;

            for (var y = 0; y < textureHeight; y++)
            {
                for (var x = 0; x < textureWidth; x++)
                {
                    //Calculate linear array index for 2d pixel position
                    var index = (y * textureWidth) + x;

                    //Tranform absolute pixel coordinate on texture to 
                    //-0.5 to +0.5 normalised range across texture dimensions

                    var fx = (x - hw) / textureWidth;
                    var fy = (y - hh) / textureHeight;

                    var distance = (float)Math.Sqrt((fx * fx) + (fy * fy));

                    if (distance > 0.5f)
                    {
                        //Bounds of wave "circles" max at width 1.0, cordners up to sqrt(2) zero
                        data[index] = 0.0f;
                    }
                    else
                    {
                        var linearFactor = distance / 0.5f; //Scales to 0 -> 1

                        var amp = maxAmplitude;

                        if (taperInner && linearFactor <= 0.5f)
                        {
                            amp *= linearFactor / 0.5f;
                        }

                        if (taperOuter && linearFactor > 0.5f)
                        {
                            amp *= 1.0f - ((linearFactor - 0.5f) / 0.5f);
                        }

                        float initAngleRads = 0.0f;
                        switch (centreWaveStartPoint)
                        {
                            case SineWavePoint.HalfPiMaxDescending:
                                initAngleRads = 0.5f * (float)Math.PI;
                                break;
                            case SineWavePoint.PiZeroDescending:
                                initAngleRads = (float)Math.PI;
                                break;
                            case SineWavePoint.OneHalfPiMaxAscending:
                                initAngleRads = 1.5f * (float)Math.PI;
                                break;
                        }

                        var totalAngleOverOscillations = 0.5f * (float)Math.PI * numHalfWaves;

                        var rads = initAngleRads + (linearFactor * totalAngleOverOscillations);

                        var value = (float)Math.Sin(rads) * amp;

                        data[index] = value;
                    }
                }
            }
            return data;
        }
    }
}