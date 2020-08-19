namespace Yak2D.Graphics
{
    public class Camera2D : ICamera2D
    {
        public ulong Id { get; private set; }

        public Camera2D(ulong id)
        {
            Id = id;
        }
    }
}