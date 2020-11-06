using System.Numerics;
using Veldrid;

namespace Yak2D.Internal
{
    public interface ICameraModel3D
    {
        ResourceSet WvpResource { get; }
        ResourceSet PositionResource { get; }

        void SetCameraView(Vector3 position, Vector3 lookAt, Vector3 up);
        void SetCameraProjection(float fovDegrees, float aspectRatio, float nearPlane, float farPlane, bool updateBuffer = true);

        void Destroy();
    }
}