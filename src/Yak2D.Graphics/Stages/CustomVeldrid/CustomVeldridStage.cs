namespace Yak2D.Graphics
{
    public class CustomVeldridStage : ICustomVeldridStage
    {
        public ulong Id { get; private set; }

        public CustomVeldridStage(ulong id)
        {
            Id = id;
        }
    }
}