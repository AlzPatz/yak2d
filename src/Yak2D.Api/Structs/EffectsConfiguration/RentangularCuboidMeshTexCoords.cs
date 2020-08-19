using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Represents the Texture Coordinates used to map a single 
    /// Texture onto a 6 sided rectangular cuboid(rectangle type cube) mesh
    ///
    /// Coordinate System is LHS, with y upwards and z positive into camera
    ///
    /// The faces are set out on the cube such that
    /// 1 to 4: side faces, with side one facing along z(into camera) for an unrotated cube mesh(up is positive y)
    ///     1 = back face, normal along positive z
    ///     2 = right face, normal along positive x
    ///     3 = front face, normal along negative z
    ///     4 = left face, normal along negative x
    /// side 5 is top face.with the top of that 2d area being furthest along positive z in the above example
    /// side 6 is bottom face. orientated so that upon rotating the above mentioned around the x axis the
    /// top and bottom face would appear the same way up when facing the camera
    /// i.e.when unrotated the top of the 2d area will be closest to camera upside down.
    /// a little tricky to describe - but should be roughly "as expected"
    ///
    /// Most use cases will use the standard / auto generated coordinates.please note
    /// the texture is split into 6 regions. 2 rows of 3, with each face assigned as:
    ///
    /// [1][2][3]
    /// [4][5][6]
    /// </summary>
    public struct RectangularCuboidMeshTexCoords
    {
        public QuadTexCoords One { get; set; }
        public QuadTexCoords Two { get; set; }
        public QuadTexCoords Three { get; set; }
        public QuadTexCoords Four { get; set; }
        public QuadTexCoords Five { get; set; }
        public QuadTexCoords Six { get; set; }

        /// <summary>
        /// Generate Standard Texture Coordinates for a Cube
        /// 
        /// [1] [2] [3]
        /// [4] [5] [6]
        ///
        ///  & within a[x]
        ///
        ///  TopLeft    -  TopRight
        ///      |            |
        ///  BottomLeft - BottomRight
        /// </summary>
        public static RectangularCuboidMeshTexCoords StandardCuboidTexCoords()
        {
            var x0 = 0.0f;
            var x1 = 1.0f / 3.0f;
            var x2 = 2.0f / 3.0f;
            var x3 = 1.0f;

            var y0 = 0.0f;
            var y1 = 0.5f;
            var y2 = 1.0f;

            /*
                [1][2][3]
                [4][5][6]

                & within a [x]

                TopLeft    -  TopRight
                   |            |
                BottomLeft - BottomRight
            */
            return new RectangularCuboidMeshTexCoords
            {
                One = new QuadTexCoords
                {
                    TopLeft = new Vector2(x0, y0),
                    TopRight = new Vector2(x1, y0),
                    BottomLeft = new Vector2(x0, y1),
                    BottomRight = new Vector2(x1, y1)
                },
                Two = new QuadTexCoords
                {
                    TopLeft = new Vector2(x1, y0),
                    TopRight = new Vector2(x2, y0),
                    BottomLeft = new Vector2(x1, y1),
                    BottomRight = new Vector2(x2, y1)
                },
                Three = new QuadTexCoords
                {
                    TopLeft = new Vector2(x2, y0),
                    TopRight = new Vector2(x3, y0),
                    BottomLeft = new Vector2(x2, y1),
                    BottomRight = new Vector2(x3, y1)
                },
                Four = new QuadTexCoords
                {
                    TopLeft = new Vector2(x0, y1),
                    TopRight = new Vector2(x1, y1),
                    BottomLeft = new Vector2(x0, y2),
                    BottomRight = new Vector2(x1, y2)
                },
                Five = new QuadTexCoords
                {
                    TopLeft = new Vector2(x1, y1),
                    TopRight = new Vector2(x2, y1),
                    BottomLeft = new Vector2(x1, y2),
                    BottomRight = new Vector2(x2, y2)
                },
                Six = new QuadTexCoords
                {
                    TopLeft = new Vector2(x2, y1),
                    TopRight = new Vector2(x3, y1),
                    BottomLeft = new Vector2(x2, y2),
                    BottomRight = new Vector2(x3, y2)
                }
            };
        }
    }

    public struct QuadTexCoords
    {
        public Vector2 TopLeft { get; set; }
        public Vector2 TopRight { get; set; }
        public Vector2 BottomLeft { get; set; }
        public Vector2 BottomRight { get; set; }
    }
}