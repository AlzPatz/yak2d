using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Mesh render stage allows multiple directional and spotlight lights
    /// The lighting model is Phong -> per-pixel lighting 
    /// </summary>
    public struct MeshRenderLightConfiguration
    {
        /// <summary>
        /// Light type - directional or spotlight
        /// </summary>
        public LightType LightType { get; set; }

        /// <summary>
        /// Directional - Represents light direction unit vector
        /// Spotlight - Represents the light's position in space
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The light's colour
        /// </summary>
        public Vector3 Colour { get; set; }

        /// <summary>
        /// Spotlight only - the attenuation factor (decresing intensity with distance)
        /// </summary>
        public float Attenuation { get; set; }

        /// <summary>
        /// Strength of ambient light contribution (scales light colour * surface colour)
        /// </summary>
        public float AmbientCoefficient { get; set; }

        /// <summary>
        /// Spotlight only
        /// </summary>
        public float ConeAngle { get; set; }

        /// <summary>
        /// Spotlight only - the direction the light is pointing
        /// </summary>
        public Vector3 ConeDirection { get; set; }
    }
}