namespace Yak2D
{
    /// <summary>
    /// Provides tools to help the generation of height map distortion textures, as well as simple distotion sprite evolution and lifecycle manager
    /// </summary>
    public interface IDistortionHelper
    {
        /// <summary>
        /// Helpers generate Textures suitable for use in distortion drawing
        /// </summary>
        IDistortionTextureGenerator TextureGenerator { get; }

        /// <summary>
        /// A collection used to manage the evolution, position, size, intensity of distortion 'sprites' (textured quads) drawn over time
        /// <summary>
        /// <param name="initialCollectionSize">Collection will be automatically doubled in size when required</param>
        IDistortionCollection CreateNewCollection(uint initialCollectionSize = 64);
    }
}