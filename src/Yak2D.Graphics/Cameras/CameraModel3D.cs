using System;
using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    /*
   Only a select few factory functions assume any handedness, like the ones involving camera and projection matrices.

   CreateOrthographic
   CreateOrthographicOffCenter
   CreatePerspective
   CreatePerspectiveOffCenter
   CreatePerspectiveFieldOfView
   CreateLookAt

   These methods will assume you are using a right-handed coordinate system (+x == Right, +y == Up, +z = Towards you).

   If your program uses a different coordinate system convention, then you can use one of the regular constructors on Matrix4x4. The rest of the library will behave the same.
   */

    public class CameraModel3D : ICameraModel3D
    {
        public ResourceSet WvpResource { get; private set; }
        public ResourceSet PositionResource { get; private set; }

        private DeviceBuffer _wvpMatrixBuffer;
        private DeviceBuffer _cameraPositionBuffer;

        private readonly ISystemComponents _components;

        private Matrix4x4 _view;
        private Matrix4x4 _projection;
        private Vector3 _cameraPosition;

        public CameraModel3D(ISystemComponents components,
                                                    Vector3 position,
                                                    Vector3 lookAt,
                                                    Vector3 up,
                                                    float fovDegrees,
                                                    float aspectRatio,
                                                    float nearPlane,
                                                    float farPlane)
        {
            _components = components;

            InitBuffers();

            SetCameraProjection(fovDegrees, aspectRatio, nearPlane, farPlane, true);
            SetCameraView(position, lookAt, up);
        }

        public void Destroy()
        {
            WvpResource.Dispose();
            PositionResource.Dispose();
            _wvpMatrixBuffer.Dispose();
            _cameraPositionBuffer.Dispose();
        }

        private void InitBuffers()
        {
            //MVP - In Vertex Shader
            _wvpMatrixBuffer = _components.Factory.CreateBuffer(new BufferDescription(MeshVertexUniforms.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            //Duped layouts from renderer
            WvpResource = _components.Factory.CreateResourceSet(
                    new ResourceSetDescription(
                        _components.Factory.CreateResourceLayout(
                            new ResourceLayoutDescription(
                                new ResourceLayoutElementDescription("VertexUniforms", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                                )), _wvpMatrixBuffer)
            );

            //CameraPosition - In Fragment Shader
            _cameraPositionBuffer = _components.Factory.CreateBuffer(new BufferDescription(FragUniforms.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            //Duped layouts from renderer
            PositionResource = _components.Factory.CreateResourceSet(
                new ResourceSetDescription(
                    _components.Factory.CreateResourceLayout(
                        new ResourceLayoutDescription(
                            new ResourceLayoutElementDescription("FragUniforms", ResourceKind.UniformBuffer, ShaderStages.Fragment)
                            )), _cameraPositionBuffer)
            );
        }

        public void SetCameraProjection(float fovDegrees, float aspectRatio, float nearPlane, float farPlane, bool updateBuffer = true)
        {
            _projection = Matrix4x4.CreatePerspectiveFieldOfView(fovDegrees * (float)Math.PI / 180f,
                                                                        aspectRatio,
                                                                        nearPlane,
                                                                        farPlane);

            if (updateBuffer)
            {
                UpdateCameraBuffers();
            }
        }

        public void SetCameraView(Vector3 position, Vector3 lookAt, Vector3 up)
        {
            _cameraPosition = position;
            _view = Matrix4x4.CreateLookAt(position, lookAt, up);

            UpdateCameraBuffers();
        }
       
        private void UpdateCameraBuffers()
        {
            var WVP = Matrix4x4.Identity * _view * _projection;

            var vertexUniforms = new MeshVertexUniforms
            {
                WorldViewProjection = WVP
            };
            _components.Device.UpdateBuffer(_wvpMatrixBuffer, 0, ref vertexUniforms);

            var fragUniforms = new FragUniforms
            {
                CameraPosition = new Vector4(_cameraPosition, 0.0f),
                Pad0 = new Vector4(0.0f)
            };

            _components.Device.UpdateBuffer(_cameraPositionBuffer, 0, ref fragUniforms);
        }
    }
}