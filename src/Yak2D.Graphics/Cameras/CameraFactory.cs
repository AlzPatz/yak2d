using System.Numerics;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class CameraFactory : ICameraFactory
    {
        private readonly ISystemComponents _components;

        public CameraFactory(ISystemComponents components)
        {
            _components = components;
        }

        public ICameraModel2D CreateCamera2D(uint virtualResolutionWidth = 960,
                                             uint virtualResolutionHeight = 540,
                                             float zoom = 1)
        {
            if (zoom <= 0.0f)
            {
                zoom = 1.0f;
            }

            return new CameraModel2D(_components,
                                     virtualResolutionWidth,
                                     virtualResolutionHeight,
                                     zoom,
                                     Vector2.Zero);
        }

        //Left Handed Coordinate System
        public ICameraModel3D CreateCamera3D(Vector3 position,
                                             Vector3 lookAt,
                                             Vector3 up,
                                             float fieldOfViewDegress = 75,
                                             float aspectRatio = 16.0f / 9.0f,
                                             float nearPlane = 0.0001f,
                                             float farPlane = 10000.0f)
        {
            if (fieldOfViewDegress <= 0.0f)
            {
                fieldOfViewDegress = 75.0f;
            }

            if (nearPlane <= 0.0f)
            {
                nearPlane = 0.0001f;
            }

            if (farPlane <= nearPlane)
            {
                farPlane = nearPlane + 1000.0f;
            }

            if (up == Vector3.Zero)
            {
                up = Vector3.UnitY;
            }

            if (position == lookAt)
            {
                lookAt = position + Vector3.UnitZ;
            }

            return new CameraModel3D(_components,
                                 position,
                                 lookAt,
                                 up,
                                 fieldOfViewDegress,
                                 aspectRatio,
                                 nearPlane,
                                 farPlane);
        }
    }
}