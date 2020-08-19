using System.Numerics;
using Veldrid;

namespace Yak2D.Graphics
{
    public interface ICameraModel2D
    {
        ResourceSet ResourceSet { get; }

        void SetWorldFocus(Vector2 focus);
        void SetWorldZoom(float zoom);
        void SetWorldRotationUsingUpVector(Vector2 up);
        void SetWorldRotationDegressClockwiseFromPositiveY(float angle);
        void SetWorldFocusAndZoom(Vector2 focus, float zoom);
        void SetWorldFocusZoomAndRotationAngleClockwiseFromPositiveY(Vector2 focus, float zoom, float angle);
        void SetWorldFocusZoomAndRotationUsingUpVector(Vector2 focus, float zoom, Vector2 up);

        float GetWorldZoom();
        Vector2 GetWorldFocus();
        float GetWorldClockwiseRotationRadsFromPositiveY();

        void SetVirtualResolution(uint width, uint height);
        Point GetVirtualResolution();

        void Destroy();
    }
}