using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Provides functions to generate the Vertex3D arrays for a selection of mesh types for the MeshRender stage
    /// </summary>
    public interface ICommonMeshBuilder
    {
        /// <summary>
        /// Creates Mesh to replicate cured scren of cathode ray monitor
        /// It's general approach to is to take a surface patch on a sphere and deform as required
        /// </summary>
        /// <param name="width">Width of Mesh at widest point"</param>
        /// <param name="height">Height of Mesg at tallest point"</param>
        /// <param name="numMeshDivisionsPerAxis">Defines the number of segments that the curved surface is divided into on each axis</param>
        /// <paran name="cornerRoundingFraction">Defines the size of curved corners in terms of a fraction of the smallest dimension. 1.0 in the instance that width==height would result in no uncurved edge portions ('circular' edge)</param>
        Vertex3D[] CreateCrtMesh(float width,
                                 float height,
                                 uint numMeshDivisionsPerAxis,
                                 float horizontalAngularCurvatureFraction,
                                 float cornerRoundingFraction);

        /// <summary>
        /// Creates a Quad Mesh from two triangles
        /// </summary>
        /// <param name="width">Width of Quad</param>
        /// <param name="height">Height of Quad</param>
        Vertex3D[] CreateQuadMesh(float width,
                                  float height);

        /// <summary>
        /// Creates a 6 sided mesh, cuboid in shape
        /// </summary>
        /// <param name="position">Position of centre of mesh</param>
        /// <param name="width">Width of mesh</param>
        /// <param name="height">Height of mesh</param>
        /// <param name="depth">Depth of mesh</param>
        /// <param name="rotationDegressAroundY">Angle in degress to rotate shape around Y (up) axis"</param>
        /// <param name="textureCoords"></param>Defines how texture coordinates are mapped onto the 6 sides</param>
        Vertex3D[] CreateRectangularCuboidMesh(Vector3 position,
                                               float width,
                                               float height,
                                               float depth,
                                               float rotationDegreesAroundY,
                                               RectangularCuboidMeshTexCoords? textureCoords = null);

        /// <summary>
        /// Creates a Spherical mesh, which is deformed / resized in each dimension.
        /// When width == height == depth, a sphere is generated
        /// </summary>
        /// <param name="position">Centre position of spherical mesh</param>
        /// <param name="width">Width of mesh shape</param>
        /// <param name="height">Height of mesh shape</param>
        /// <param name="depth">Depth of mesh shape</param>
        /// <param name="rotationDegreesClockwiseYAxis">Angle in degress to rotate shape around Y (up) axis"</param>
        /// <param name="numDivsHorizontal">Defines the number of segments that the curved surface is divided into around the spherical shapes equator</param>
        /// <param name="numDivVertical">Defines the number of segments that the curved surface is divided into from pole to pole</param>
        Vertex3D[] CreateSphericalMesh(Vector3 position,
                                       float width,
                                       float height,
                                       float depth,
                                       float rotationDegreesClockwiseYAxis,
                                       uint numDivsHorizontal,
                                       uint numDivsVertical);
    }
}