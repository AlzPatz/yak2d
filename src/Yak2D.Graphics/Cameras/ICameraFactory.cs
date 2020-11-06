using System.Numerics;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface ICameraFactory
    {
        ICameraModel2D CreateCamera2D(uint virtualResolutionWidth,
                                      uint virtualResolutionHeight,
                                      float zoom);

        //Left Handed Coordinate System
        ICameraModel3D CreateCamera3D(Vector3 position,
                                      Vector3 lookAt,
                                      Vector3 up,
                                      float fieldOfViewDegress,
                                      float aspectRation,
                                      float nearPlane,
                                      float farPlane);
    }
}