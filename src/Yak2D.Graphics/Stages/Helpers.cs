namespace Yak2D.Graphics
{
    public class Helpers : IHelpers
    {
        public ICoordinateTransforms CoordinateTransforms { get; private set; }
        public ICommonMeshBuilder CommonMeshBuilder { get; private set; }
        public IDistortionHelper DistortionHelper { get; private set; }

        public Helpers(ICoordinateTransforms coordinateTransforms,
                        ICommonMeshBuilder commonMeshBuilder,
                        IDistortionHelper distortionHelper)
        {
            CoordinateTransforms = coordinateTransforms;
            CommonMeshBuilder = commonMeshBuilder;
            DistortionHelper = distortionHelper;
        }
    }
}