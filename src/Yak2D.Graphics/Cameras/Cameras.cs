using System.Numerics;

namespace Yak2D.Graphics
{
    public class Cameras : ICameras
    {
        //Mostly pass through now to camera manager, perhaps just collapse and remove a layer
        private ICameraManager _cameraManager;

        public int Camera2DCount => _cameraManager.Count2D;
        public int Camera3DCount => _cameraManager.Count3D;

        public Cameras(ICameraManager cameraManager)
        {
            _cameraManager = cameraManager;
        }

        public ICamera2D CreateCamera2D(uint virtualResolutionWidth = 960,
                         uint virtualResolutionHeight = 540,
                         float zoom = 1.0f) =>
                            _cameraManager.CreateCamera2D(virtualResolutionWidth,
                                                          virtualResolutionHeight,
                                                          zoom);

        //Left Handed Coordinate System
        public ICamera3D CreateCamera3D(Vector3 position,
                                 Vector3 lookAt,
                                 Vector3 up,
                                 float fieldOfViewDegress = 60.0f,
                                 float aspectRation = 16.0f / 9.0f,
                                 float nearPlane = 0.0001f,
                                 float farPlane = 1000.0f) =>
                                    _cameraManager.CreateCamera3D(position,
                                                                  lookAt,
                                                                  up,
                                                                  fieldOfViewDegress,
                                                                  aspectRation,
                                                                  nearPlane,
                                                                  farPlane);

        public void SetCamera2DFocusAndZoom(ICamera2D camera, Vector2 focus, float zoom)
        {
            if (camera == null)
            {
                return;
            }

            _cameraManager.RetrieveCameraModel2D(camera.Id)?.SetWorldFocusAndZoom(focus, zoom);
        }

        public void SetCamera2DFocusAndZoom(ulong camera, Vector2 focus, float zoom)
        {
            _cameraManager.RetrieveCameraModel2D(camera)?.SetWorldFocusAndZoom(focus, zoom);
        }

        public void SetCamera2DWorldRotationUsingUpVector(ICamera2D camera, Vector2 up)
        {
            if (camera == null)
            {
                return;
            }

            _cameraManager.RetrieveCameraModel2D(camera.Id)?.SetWorldRotationUsingUpVector(up);
        }

        public void SetCamera2DWorldRotationUsingUpVector(ulong camera, Vector2 up)
        {
            _cameraManager.RetrieveCameraModel2D(camera)?.SetWorldRotationUsingUpVector(up);
        }

        public void SetCamera2DWorldRotationDegressClockwiseFromPositiveY(ICamera2D camera, float angle)
        {
            if (camera == null)
            {
                return;
            }

            _cameraManager.RetrieveCameraModel2D(camera.Id)?.SetWorldRotationDegressClockwiseFromPositiveY(angle);
        }

        public void SetCamera2DWorldRotationDegressClockwiseFromPositiveY(ulong camera, float angle)
        {
            _cameraManager.RetrieveCameraModel2D(camera)?.SetWorldRotationDegressClockwiseFromPositiveY(angle);
        }

        public void SetCamera2DWorldFocusZoomAndRotationAngleClockwiseFromPositiveY(ICamera2D camera, Vector2 focus, float zoom, float angle)
        {
            if (camera == null)
            {
                return;
            }

            _cameraManager.RetrieveCameraModel2D(camera.Id)?.SetWorldFocusZoomAndRotationAngleClockwiseFromPositiveY(focus, zoom, angle);
        }

        public void SetCamera2DWorldFocusZoomAndRotationAngleClockwiseFromPositiveY(ulong camera, Vector2 focus, float zoom, float angle)
        {
            _cameraManager.RetrieveCameraModel2D(camera)?.SetWorldFocusZoomAndRotationAngleClockwiseFromPositiveY(focus, zoom, angle);
        }

        public void SetCamera2DWorldFocusZoomAndRotationUsingUpVector(ICamera2D camera, Vector2 focus, float zoom, Vector2 up)
        {
            if (camera == null)
            {
                return;
            }

            _cameraManager.RetrieveCameraModel2D(camera.Id)?.SetWorldFocusZoomAndRotationUsingUpVector(focus, zoom, up);
        }

        public void SetCamera2DWorldFocusZoomAndRotationUsingUpVector(ulong camera, Vector2 focus, float zoom, Vector2 up)
        {
            _cameraManager.RetrieveCameraModel2D(camera)?.SetWorldFocusZoomAndRotationUsingUpVector(focus, zoom, up);
        }

        public void SetCamera2DVirtualResolution(ICamera2D camera, uint width, uint height)
        {
            if (camera == null)
            {
                return;
            }

            _cameraManager.RetrieveCameraModel2D(camera.Id)?.SetVirtualResolution(width, height);
        }

        public void SetCamera2DVirtualResolution(ulong camera, uint width, uint height)
        {
            _cameraManager.RetrieveCameraModel2D(camera)?.SetVirtualResolution(width, height);
        }

        public void SetCamera3DView(ICamera3D camera, Vector3 position, Vector3 lookAt, Vector3 up)
        {
            if (camera == null)
            {
                return;
            }

            _cameraManager.RetrieveCameraModel3D(camera.Id)?.SetCameraView(position, lookAt, up);
        }

        public void SetCamera3DView(ulong camera, Vector3 position, Vector3 lookAt, Vector3 up)
        {
            _cameraManager.RetrieveCameraModel3D(camera)?.SetCameraView(position, lookAt, up);
        }

        public void SetCamera3DProjection(ICamera3D camera, float fovDegrees, float aspectRatio, float nearPlane, float farPlane)
        {
            if (camera == null)
            {
                return;
            }

            _cameraManager.RetrieveCameraModel3D(camera.Id)?.SetCameraProjection(fovDegrees, aspectRatio, nearPlane, farPlane);
        }
        
        public void SetCamera3DProjection(ulong camera, float fovDegrees, float aspectRatio, float nearPlane, float farPlane)
        {
            _cameraManager.RetrieveCameraModel3D(camera)?.SetCameraProjection(fovDegrees, aspectRatio, nearPlane, farPlane);
        }

        public void DestroyCamera(ICamera camera)
        {
            if (camera == null)
            {
                return;
            }

            _cameraManager.DestroyCamera(camera.Id);
        }

        public void DestroyCamera(ulong camera)
        {
            _cameraManager.DestroyCamera(camera);
        }

        public void DestroyAllCameras()
        {
            _cameraManager.DestroyAllCameras();
        }

        public void DestroyAllCameras2D()
        {
            _cameraManager.DestroyAllCameras2D();
        }

        public void DestroyAllCameras3D()
        {
            _cameraManager.DestroyAllCameras3D();
        }
    }
}