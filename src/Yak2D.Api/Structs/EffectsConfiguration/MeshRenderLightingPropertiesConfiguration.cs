using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Mesh render stage allows multiple directional and spotlight lights
    /// The lighting model is Phong -> per-pixel lighting 
    /// </summary>
    public struct MeshRenderLightingPropertiesConfiguration
    {
        /// <summary>
        /// Specular highlight colour is per-mesh, applies to all lights
        /// </summary>
        public Vector3 SpecularColour { get; set; }

        /// <summary>
        /// Shininess is per-mesh, applies to all lights
        /// Must be >= 1.0f
        /// </summary>
        public float Shininess { get; set; }

        /// <summary>
        /// Number of lights active. Max number of lights == 8
        /// </summary>
        public int NumberOfActiveLights { get; set; }
    }
}