namespace Yak2D
{
    /// <summary>
    /// The Light type used in MeshRender Stages (Phong lighting model)
    /// </summary>
    public enum LightType
    {
        // <summary>
        /// A uniform direction, constant intensity light source       
        /// </summary>
        Directional,
        
        /// <summary>
        /// A light that has a source position, a direction, a defined cone shape and attenuation (drop off) over distance
        /// </summary>
        Spotlight
    }
}