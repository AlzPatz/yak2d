using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class CameraModel2D : ICameraModel2D
    {
        public ResourceSet ResourceSet { get; set; }

        private readonly ISystemComponents _components;

        private DeviceBuffer _worldViewProjectionBuffer;

        private float _zoom;
        private Vector2 _worldCameraFocus;
        private float _worldCameraRotationRads;
        private uint _virtualResolutionX;
        private uint _virtualResolutionY;

        public CameraModel2D(ISystemComponents components,
                                uint initVirtualResolutionX,
                                uint initVirtualResolutionY,
                                float initialZoom,
                                Vector2 initialWorldFocusPosition)
        {
            _components = components;

            _worldViewProjectionBuffer = components.Factory.CreateBuffer(new BufferDescription(128, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var resourceLayout = components.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                )
            );

            ResourceSet = components.Factory.CreateResourceSet(
                    new ResourceSetDescription(resourceLayout, _worldViewProjectionBuffer)
            );

            SetWorldFocusAndZoom(initialWorldFocusPosition, initialZoom);
            SetWorldRotationDegressClockwiseFromPositiveY(0.0f);
            SetResolution(initVirtualResolutionX, initVirtualResolutionY);

            UpdateScreenMatrix();
            UpdateWorldMatrix();
        }

        public void Destroy()
        {
            ResourceSet.Dispose();
            _worldViewProjectionBuffer.Dispose();
        }

        public void SetWorldFocus(Vector2 focus)
        {
            SetFocus(focus);
            UpdateWorldMatrix();
        }

        public void SetWorldZoom(float zoom)
        {
            SetZoom(zoom);
            UpdateWorldMatrix();
        }

        public void SetWorldRotationUsingUpVector(Vector2 up)
        {
            SetRotationUsingUp(up);
            UpdateWorldMatrix();
        }

        public void SetWorldRotationDegressClockwiseFromPositiveY(float angle)
        {
            SetRotationUsingAngle(angle);
            UpdateWorldMatrix();
        }

        public void SetWorldFocusAndZoom(Vector2 focus, float zoom)
        {
            SetZoom(zoom);
            SetFocus(focus);
            UpdateWorldMatrix();
        }

        public void SetWorldFocusZoomAndRotationAngleClockwiseFromPositiveY(Vector2 focus, float zoom, float angle)
        {
            SetFocus(focus);
            SetZoom(zoom);
            SetRotationUsingAngle(angle);
            UpdateWorldMatrix();
        }

        public void SetWorldFocusZoomAndRotationUsingUpVector(Vector2 focus, float zoom, Vector2 up)
        {
            SetFocus(focus);
            SetZoom(zoom);
            SetRotationUsingUp(up);
            UpdateWorldMatrix();
        }

        private void SetZoom(float zoom)
        {
            if (zoom > 0.0f)
            {
                _zoom = zoom;
            }
        }

        private void SetFocus(Vector2 focus)
        {
            _worldCameraFocus = focus;
        }

        private void SetRotationUsingUp(Vector2 up)
        {
            _worldCameraRotationRads = -((float)System.Math.Atan2(up.Y, up.X) - (0.5f * (float)System.Math.PI));
        }

        private void SetRotationUsingAngle(float angle)
        {
            _worldCameraRotationRads = (1.0f / 180.0f) * (float)System.Math.PI * angle;
        }

        public float GetWorldZoom()
        {
            return _zoom;
        }

        public Vector2 GetWorldFocus()
        {
            return _worldCameraFocus;
        }

        public float GetWorldClockwiseRotationRadsFromPositiveY()
        {
            return _worldCameraRotationRads;
        }

        public void SetVirtualResolution(uint width, uint height)
        {
            SetResolution(width, height);
            UpdateScreenMatrix();
            UpdateWorldMatrix();
        }

        private void SetResolution(uint width, uint height)
        {
            _virtualResolutionX = width;
            _virtualResolutionY = height;
        }

        public Point GetVirtualResolution()
        {
            return new Point((int)_virtualResolutionX, (int)_virtualResolutionY);
        }

        private void UpdateScreenMatrix()
        {
            var orthographicProjection = Matrix4x4.CreateOrthographic(_virtualResolutionX, _virtualResolutionY, 0.0f, 1.0f);
            _components.Device.UpdateBuffer(_worldViewProjectionBuffer, 64, ref orthographicProjection);
        }

        private void UpdateWorldMatrix()
        {
            var width = _virtualResolutionX / _zoom;
            var height = _virtualResolutionY / _zoom;
            CalculateWorldMatrix(width, height, 0.0f, 1.0f);
        }

        private void CalculateWorldMatrix(float viewWidth, float viewHeight, float nearPlane, float farPlane)
        {
            var orthographicProjection = Matrix4x4.CreateOrthographic(viewWidth, viewHeight, nearPlane, farPlane);

            var translation = Matrix4x4.CreateTranslation(new Vector3(-_worldCameraFocus, 0.0f));
            var rotation = Matrix4x4.CreateRotationZ(_worldCameraRotationRads);

            var worldModelViewProjection = Matrix4x4.Multiply(Matrix4x4.Multiply(translation, rotation), orthographicProjection);

            _components.Device.UpdateBuffer(_worldViewProjectionBuffer, 0, ref worldModelViewProjection);
        }
    }
}