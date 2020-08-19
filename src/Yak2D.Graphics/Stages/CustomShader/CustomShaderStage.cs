namespace Yak2D.Graphics
{
    public class CustomShaderStage : ICustomShaderStage
    {
        public ulong Id { get; private set; }

        public CustomShaderStage(ulong id)
        {
            Id = id;
        }
    }
}