namespace Yak2D.Graphics
{
    public class Helpers : IHelpers
    {
        public ICommonMeshBuilder CommonMeshBuilder { get; private set; }
        public IDistortionHelper DistortionHelper { get; private set; }

        public Helpers(ICommonMeshBuilder commonMeshBuilder,
                        IDistortionHelper distortionHelper)
        {
            CommonMeshBuilder = commonMeshBuilder;
            DistortionHelper = distortionHelper;
        }
    }
}