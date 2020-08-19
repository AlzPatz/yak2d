namespace Yak2D.Graphics
{
    public class DistortionHelper : IDistortionHelper
    {
        public IDistortionTextureGenerator TextureGenerator { get; private set; }

        public DistortionHelper(IDistortionTextureGenerator textureGenerator)
        {
            TextureGenerator = textureGenerator;
        }

        public IDistortionCollection CreateNewCollection(uint initialCollectionSize = 64)
        {
            return new DistortionCollection(initialCollectionSize);
        }
    }
}