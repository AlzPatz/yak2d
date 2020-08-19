namespace Yak2D.Graphics
{
    public class Camera3D : ICamera3D
    {
        public ulong Id { get; private set; }

        public Camera3D(ulong id)
        {
            Id = id;
        }
    }
}