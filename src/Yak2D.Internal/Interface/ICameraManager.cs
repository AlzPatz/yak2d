using System.Numerics;

namespace Yak2D.Internal
{
    public interface ICameraManager
    {
        int Count2D { get; }
        int Count3D { get; }

        ICamera2D CreateCamera2D(uint virtualResolutionWidth,
                                 uint virtualResolutionHeight,
                                 float zoom);

        ICamera3D CreateCamera3D(Vector3 position,
                                 Vector3 lookAt,
                                 Vector3 up,
                                 float fieldOfViewDegress,
                                 float aspectRation,
                                 float nearPlane,
                                 float farPlane);

        ICameraModel2D RetrieveCameraModel2D(ulong key);
        ICameraModel3D RetrieveCameraModel3D(ulong key);

        void DestroyCamera(ulong id);
        void DestroyAllCameras();
        void DestroyAllCameras2D();
        void DestroyAllCameras3D();
        void Shutdown();
    }
}