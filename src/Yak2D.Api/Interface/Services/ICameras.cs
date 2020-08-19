using System.Numerics;

namespace Yak2D
{
    /// <summary>
	/// Camera Operations: Creation, Configuration and Destruction
    /// 2D Cameras are used by Drawing Stages (IDrawStage and IDistortionStage)
    /// 3D Cameras are used by the MeshRender Stage
	/// </summary>
    public interface ICameras
    {
        /// <summary>
        /// Returns the number of 2D Cameras that currently exist
        /// </summary>
        int Camera2DCount { get; }
        
        /// <summary>
        /// Returns the number of 3D Cameras that currently exist
        /// </summary>        
        int Camera3DCount { get; }

        /// <summary>
        /// Create a Camera for use in 2D drawing operations
        /// </summary>
        /// <param name="virtualResolutionWidth">The Camera's view window 'pixel' width in terms of the global coordinate system</param>
        /// <param name="virtualResolutionHeight">The Camera's view window 'pixel' height in terms of the global coordinate system</param>
        /// <param name="zoom">The Camera's view window world-view zoom factor</param>
        ICamera2D CreateCamera2D(uint virtualResolutionWidth = 960,
                                 uint virtualResolutionHeight = 540,
                                 float zoom = 1.0f);
        /// <summary>
        /// Create a Camera for use in 3D rendering operations. Left-handed coordinate system
        /// </summary>
        /// <param name="position">Camera's position in 3D space</param>
        /// <param name="lookAt">A point in 3D space that the camera is looking at</param>
        /// <param name="up">A unit vector describing the direction that is 'up' relative to the Camera view</param>
        /// <param name="fieldOfViewDegress">Horizontal field of view angle in degress"</param>
        /// <param name="aspectRatio">Aspect ratio of camera's viewport (width / height)"</param>
        /// <param name="nearPlane">Closest distance to camera in 3D space that an object would be rendered"</param>
        /// <param name="farPlane">Furthest distance to camera in 3D space that an object would be rendered"</param>
        ICamera3D CreateCamera3D(Vector3 position,
                                 Vector3 lookAt,
                                 Vector3 up,
                                 float fieldOfViewDegress = 60.0f,
                                 float aspectRatio = 16.0f / 9.0f,
                                 float nearPlane = 0.0001f,
                                 float farPlane = 1000.0f);

        /// <summary>
        /// Set a Camera's view window dimensions in terms of the global coordinate system
        /// </summary>
        /// <param name="camera">The camera to modify</param>
        /// <param name="width">The camera's width in terms of the global coordinate system</param>
        /// <param name="height">The camera's height in terms of the global coordinate system</param>
        void SetCamera2DVirtualResolution(ICamera2D camera, uint width, uint height);
        /// <summary>
        /// Set a Camera's view window dimensions in terms of the global coordinate system
        /// </summary>
        /// <param name="camera">The id key of the camera to modify</param>
        /// <param name="width">The camera's width in terms of the global coordinate system</param>
        /// <param name="height">The camera's height in terms of the global coordinate system</param>
        void SetCamera2DVirtualResolution(ulong camera, uint width, uint height);

        /// <summary>
        /// Set a Camera's world focus position and zoom
        /// </summary>
        /// <param name="camera">The camera to modify</param>
        /// <param name="focus">The world position to use as the camera's focus point</param>
        /// <param name="zoom">The camera's world zoom factor</param>
        void SetCamera2DFocusAndZoom(ICamera2D camera, Vector2 focus, float zoom);
        /// <summary>
        /// Set a Camera's world focus position and zoom
        /// </summary>
        /// <param name="camera">The id key of the camera to modify</param>
        /// <param name="focus">The world position to use as the camera's focus point</param>
        /// <param name="zoom">The camera's world zoom factor</param>
        void SetCamera2DFocusAndZoom(ulong camera, Vector2 focus, float zoom);

        /// <summary>
        /// Set a Camera's rotation described by an 'up' vector
        /// </summary>
        /// <param name="camera">The camera to modify</param>
        /// <param name="up">A unit vector pointing in the upwards direction of the camera</param>
        void SetCamera2DWorldRotationUsingUpVector(ICamera2D camera, Vector2 up);
        /// <summary>
        /// Set a Camera's rotation described by an 'up' vector
        /// </summary>
        /// <param name="camera">The id key of the camera to modify</param>
        /// <param name="up">A unit vector pointing in the upwards direction of the camera</param>
        void SetCamera2DWorldRotationUsingUpVector(ulong camera, Vector2 up);        
        
        /// <summary>
        /// Set a Camera's rotation as clockwise rotation from the positive Y axis direction
        /// </summary>
        /// <param name="camera">The camera to modify</param>
        /// <param name="angle">Angle in degress representing a clockwise rotation from the positive Y axis direction</param>
        void SetCamera2DWorldRotationDegressClockwiseFromPositiveY(ICamera2D camera, float angle);
        /// <summary>
        /// Set a Camera's rotation as clockwise rotation from the positive Y axis direction
        /// </summary>
        /// <param name="camera">The id key of the camera to modify</param>
        /// <param name="angle">Angle in degress representing a clockwise rotation from the positive Y axis direction</param>
        void SetCamera2DWorldRotationDegressClockwiseFromPositiveY(ulong camera, float angle);

        /// <summary>
        /// Set a Camera's world focus position, zoom and rotation described as a clockwise rotation from the positive Y axis direction
        /// </summary>
        /// <param name="camera">The camera to modify</param>
        /// <param name="focus">The world position to use as the camera's focus point</param>
        /// <param name="zoom">The camera's world zoom factor</param>
        /// <param name="angle">Angle in degress representing a clockwise rotation from the positive Y axis direction</param>
        void SetCamera2DWorldFocusZoomAndRotationAngleClockwiseFromPositiveY(ICamera2D camera, Vector2 focus, float zoom, float angle);
        /// <summary>
        /// Set a Camera's world focus position, zoom and rotation described as a clockwise rotation from the positive Y axis direction
        /// </summary>
        /// <param name="camera">The id key of the camera to modify</param>
        /// <param name="focus">The world position to use as the camera's focus point</param>
        /// <param name="zoom">The camera's world zoom factor</param>
        /// <param name="angle">Angle in degress representing a clockwise rotation from the positive Y axis direction</param>
        void SetCamera2DWorldFocusZoomAndRotationAngleClockwiseFromPositiveY(ulong camera, Vector2 focus, float zoom, float angle);

        /// <summary>
        /// Set a Camera's world focus position, zoom and rotation described by an 'up' vector
        /// </summary>
        /// <param name="camera">The camera to modify</param>
        /// <param name="focus">The world position to use as the camera's focus point</param>
        /// <param name="zoom">The camera's world zoom factor</param>
        /// <param name="up">A unit vector pointing in the upwards direction of the camera</param>
        void SetCamera2DWorldFocusZoomAndRotationUsingUpVector(ICamera2D camera, Vector2 focus, float zoom, Vector2 up);
        /// <summary>
        /// Set a Camera's world focus position, zoom and rotation described by an 'up' vector
        /// </summary>
        /// <param name="camera">The id key of the camera to modify</param>
        /// <param name="focus">The world position to use as the camera's focus point</param>
        /// <param name="zoom">The camera's world zoom factor</param>
        /// <param name="up">A unit vector pointing in the upwards direction of the camera</param>
        void SetCamera2DWorldFocusZoomAndRotationUsingUpVector(ulong camera, Vector2 focus, float zoom, Vector2 up);

        /// <summary>
        /// Set a Camera's 3D view orientation
        /// </summary>
        /// <param name="camera">The camera to modify</param>
        /// <param name="position">Camera's position in 3D space</param>
        /// <param name="lookAt">A point in 3D space that the camera is looking at</param>
        /// <param name="up">A unit vector describing the direction that is 'up' relative to the Camera view</param>
        void SetCamera3DView(ICamera3D camera, Vector3 position, Vector3 lookAt, Vector3 up);
        /// <summary>
        /// Set a Camera's 3D view orientation
        /// </summary>
        /// <param name="camera">The id key of the camera to modify</param>
        /// <param name="position">Camera's position in 3D space</param>
        /// <param name="lookAt">A point in 3D space that the camera is looking at</param>
        /// <param name="up">A unit vector describing the direction that is 'up' relative to the Camera view</param>
        void SetCamera3DView(ulong camera, Vector3 position, Vector3 lookAt, Vector3 up);

        /// <summary>
        /// Set a Camera's 3D view projection
        /// </summary>
        /// <param name="camera">The camera to modify</param>
        /// <param name="fovDegress">Horizontal field of view angle in degress"</param>
        /// <param name="aspectRatio">Aspect ratio of camera's viewport (width / height)"</param>
        /// <param name="nearPlane">Closest distance to camera in 3D space that an object would be rendered"</param>
        /// <param name="farPlane">Furthest distance to camera in 3D space that an object would be rendered"</param>
        void SetCamera3DProjection(ICamera3D camera, float fovDegress, float aspectRatio, float nearPlane, float farPlane);
        /// <summary>
        /// Set a Camera's 3D view projection
        /// </summary>
        /// <param name="camera">The id key of the camera to modify</param>
        /// <param name="fovDegress">Horizontal field of view angle in degress"</param>
        /// <param name="aspectRatio">Aspect ratio of camera's viewport (width / height)"</param>
        /// <param name="nearPlane">Closest distance to camera in 3D space that an object would be rendered"</param>
        /// <param name="farPlane">Furthest distance to camera in 3D space that an object would be rendered"</param>
        void SetCamera3DProjection(ulong camera, float fovDegress, float aspectRatio, float nearPlane, float farPlane);

        /// <summary>
        /// Destroy all cameras (2D and 3D)
        /// </summary>
        void DestroyAllCameras();
        
        /// <summary>
        /// Destroy all 2D cameras
        /// </summary>
        void DestroyAllCameras2D();

        /// <summary>
        /// Destroy all 3D cameras
        /// </summary>       
        void DestroyAllCameras3D();

        /// <summary>
        /// Destroy an existing Camera
        /// </summary>
        /// <param name="camera">The Camera to destory"</param>
        void DestroyCamera(ICamera camera);
        /// <summary>
        /// Destroy an existing Camera
        /// </summary>
        /// <param name="camera">The id key of the Camera to destory"</param>
        void DestroyCamera(ulong camera);    }
}