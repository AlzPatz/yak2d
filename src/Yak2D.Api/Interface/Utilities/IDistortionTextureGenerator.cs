namespace Yak2D
{
    /// <summary>
    /// Generate Textures suitable for use in distortion drawing (Rgba and Float32)
    /// </summary>
    public interface IDistortionTextureGenerator
    {
        /// <summary>
        /// Generates a texture of concentric, spherical sinusodial waves
        /// The texture's pixels are four component RGBA
        /// </summary>
        /// <param name="textureWidth">Width in pixels</param>
        /// <param name="textureHeight">Height in pixels</param>
        /// <param name="numQuarterWaves">A quarter wave is 90 degress / PI radians of a sinusodial curve</param>
        /// <param name="taperInner">Intensity will be scaled from 0 in the centre to 1.0 at the mid radius distance</param>
        /// <param name="taperOuter">Intensity will be scaled from 1.0 at the mid radius distance to 0 at the outer radius distance</param>
        /// <param name="centreWaveStartPoint">Set where on the sinusodial curve the wave oscillations start</param>
        /// <param name="maxAmplitude">Amplitude of sinusodial wave</param>
        ITexture ConcentricSinusoidalRgba(uint textureWidth,
                                          uint textureHeight,
                                          int numQuarterWaves,
                                          bool taperInner,
                                          bool taperOuter,
                                          SineWavePoint centreWaveStartPoint = SineWavePoint.ZeroAscending,
                                          float maxAmplitude = 1.0f);
        
        /// <summary>
        /// Generates a texture of concentric, spherical sinusodial waves
        /// The texture's pixels are single component 32 floats
        /// </summary>
        /// <param name="textureWidth">Width in pixels</param>
        /// <param name="textureHeight">Height in pixels</param>
        /// <param name="numQuarterWaves">A quarter wave is 90 degress / PI radians of a sinusodial curve</param>
        /// <param name="taperInner">Intensity will be scaled from 0 in the centre to 1.0 at the mid radius distance</param>
        /// <param name="taperOuter">Intensity will be scaled from 1.0 at the mid radius distance to 0 at the outer radius distance</param>
        /// <param name="centreWaveStartPoint">Set where on the sinusodial curve the wave oscillations start</param>
        /// <param name="maxAmplitude">Amplitude of sinusodial wave</param>
        ITexture ConcentricSinusoidalFloat32(uint textureWidth,
                                             uint textureHeight,
                                             int numQuarterWaves,
                                             bool taperInner,
                                             bool taperOuter,
                                             SineWavePoint centreWaveStartPoint = SineWavePoint.ZeroAscending,
                                             float maxAmplitude = 1.0f);
    }
}