using System.Numerics;

namespace Yak2D.Graphics
{
    public class QuadMeshBuilder : IQuadMeshBuilder
    {
        public Vertex3D[] Build(float width, float height)
        {
            if (width <= 0.0f)
            {
                width = 1.0f;
            }

            if (height <= 0.0f)
            {
                height = 1.0f;
            }

            var hw = 0.5f * width;
            var hh = 0.5f * height;

            //Framework uses a Right Handed Coordinate System in 3D (x positive to the right, y positive upwards, z positive towards camera)
            //Quad on X-Y plane, normal away from plane is positive z direction
            var mesh = new Vertex3D[]
            {
                new Vertex3D
                {
                    Position = new Vector3(-hw, hh, 0.0f),
                    Normal = Vector3.UnitZ,
                    TexCoord = new Vector2(0.0f, 0.0f)
                },
                new Vertex3D
                {
                    Position = new Vector3(hw, hh, 0.0f),
                    Normal = Vector3.UnitZ,
                    TexCoord = new Vector2(1.0f, 0.0f)
                },
                new Vertex3D
                {
                    Position = new Vector3(-hw, -hh, 0.0f),
                    Normal = Vector3.UnitZ,
                    TexCoord = new Vector2(0.0f, 1.0f)
                },
                new Vertex3D
                {
                    Position = new Vector3(-hw, -hh, 0.0f),
                    Normal = Vector3.UnitZ,
                    TexCoord = new Vector2(0.0f, 1.0f)
                },
                new Vertex3D
                {
                    Position = new Vector3(hw, hh, 0.0f),
                    Normal = Vector3.UnitZ,
                    TexCoord = new Vector2(1.0f, 0.0f)
                },
                new Vertex3D
                {
                    Position = new Vector3(hw, -hh, 0.0f),
                    Normal = Vector3.UnitZ,
                    TexCoord = new Vector2(1.0f, 1.0f)
                }
            };

            return mesh;
        }
    }
}