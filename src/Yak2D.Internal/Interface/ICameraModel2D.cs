using System.Numerics;
using Veldrid;

namespace Yak2D.Internal
{
    public interface ICameraModel2D
    {
        ResourceSet ResourceSet { get; }

        void SetWorldFocus(Vector2 focus);
        void SetWorldZoom(float zoom);
        void SetWorldRotationUsingUpVector(Vector2 up);
        void SetWorldRotationRadiansClockwiseFromPositiveY(float angle);
        void SetWorldFocusAndZoom(Vector2 focus, float zoom);
        void SetWorldFocusZoomAndRotationRadiansAngleClockwiseFromPositiveY(Vector2 focus, float zoom, float angle);
        void SetWorldFocusZoomAndRotationUsingUpVector(Vector2 focus, float zoom, Vector2 up);

        float GetWorldZoom();
        Vector2 GetWorldFocus();
        float GetWorldClockwiseRotationRadsFromPositiveY();
        Vector2 GetWorldUp();

        void SetVirtualResolution(uint width, uint height);
        Point GetVirtualResolution();

        void Destroy();
    }
}