namespace Yak2D
{
    /// <summary>
	/// Helper objects to support / simplify rendering stages
	/// </summary>
    public interface IHelpers
    {
        /// <summary>
        /// Provides functions to generate the Vertex3D arrays for a selection of mesh types for the MeshRender stage
        /// </summary>
        ICommonMeshBuilder CommonMeshBuilder { get; }

        /// <summary>
        /// Provides tools to help the generation of height map distortion textures, as well as simple distotion sprite evolution and lifecycle manager
        /// </summary>
        IDistortionHelper DistortionHelper { get; }
    }
}