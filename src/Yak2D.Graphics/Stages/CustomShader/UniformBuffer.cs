using Veldrid;

namespace Yak2D.Graphics
{
    public class UniformBuffer
    {
        public string Name { get; set; }
        public DeviceBuffer Buffer { get; set; }
        public ResourceSet ResouceSet { get; set; }
    }
}