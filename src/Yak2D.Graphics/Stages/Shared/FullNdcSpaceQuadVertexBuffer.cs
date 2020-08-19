using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class FullNdcSpaceQuadVertices : IFullNdcSpaceQuadVertexBuffer
    {
        private readonly ISystemComponents _systemComponents;

        public DeviceBuffer Buffer { get; private set; }

        public FullNdcSpaceQuadVertices(ISystemComponents systemComponents)
        {
            _systemComponents = systemComponents;

            Create();
        }

        private void Create()
        {
            VertexTextured2D[] fullNdcSpaceQuadVertices =
            {
                    new VertexTextured2D { Position = new Vector2(-1.0f, 1.0f), TexCoord = new Vector2(0.0f, 0.0f) },
                    new VertexTextured2D { Position = new Vector2(1.0f, 1.0f), TexCoord = new Vector2(1.0f, 0.0f) },
                    new VertexTextured2D { Position = new Vector2(1.0f, -1.0f), TexCoord = new Vector2(1.0f, 1.0f) },

                    new VertexTextured2D { Position = new Vector2(-1.0f, 1.0f), TexCoord = new Vector2(0.0f, 0.0f) },
                    new VertexTextured2D { Position = new Vector2(1.0f, -1.0f), TexCoord = new Vector2(1.0f, 1.0f) },
                    new VertexTextured2D { Position = new Vector2(-1.0f, -1.0f), TexCoord = new Vector2(0.0f, 1.0f) }
            };

            Buffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(6 * VertexTextured2D.SizeInBytes, BufferUsage.VertexBuffer));

            _systemComponents.Device.UpdateBuffer(Buffer, 0, fullNdcSpaceQuadVertices);
        }

        public void ReInitialise()
        {
            Create();
        }
    }
}