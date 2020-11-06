using NSubstitute;
using System;
using System.Numerics;
using Veldrid;
using Xunit;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class CameraModel2DTest
    {
        [Fact]
        public void Camera2DTest_CreateCamera_TestSettingFocusAndAngleCalculationFromVector()
        {
            var components = Substitute.For<ISystemComponents>();

            var camera = new CameraModel2D(components, 1920, 1080, 1.0f, Vector2.Zero);

            camera.SetWorldFocus(new Vector2(100.0f, 150.0f));
            Assert.Equal(new Vector2(100.0f, 150.0f), camera.GetWorldFocus());

            camera.SetWorldFocusZoomAndRotationUsingUpVector(new Vector2(200.0f, 250.0f), 1.0f, Vector2.UnitX);
            Assert.Equal(new Vector2(200.0f, 250.0f), camera.GetWorldFocus());
            Assert.Equal(0.5f * (float)Math.PI, camera.GetWorldClockwiseRotationRadsFromPositiveY());

        }

        [Fact]
        public void Camera2DTest_CreateCamera_EnsureNegativeIgnored()
        {
            var components = Substitute.For<ISystemComponents>();

            var camera = new CameraModel2D(components, 1920, 1080, 5.0f, Vector2.Zero);

            camera.SetWorldZoom(-0.1f);
            Assert.Equal(5.0f, camera.GetWorldZoom());
        }

        [Fact]
        public void Camera2DTest_EnsureGetVirtualResolutionWorks()
        {
            var components = Substitute.For<ISystemComponents>();

            var camera = new CameraModel2D(components, 1920, 1080, 5.0f, Vector2.Zero);

            camera.SetVirtualResolution(128, 256);
            var resolution = camera.GetVirtualResolution();
            Assert.Equal(128, resolution.X);
            Assert.Equal(256, resolution.Y);
        }

        [Fact]
        public void Camera2DTest_EnsureGetUpVectorIsCorrect()
        {
            var components = Substitute.For<ISystemComponents>();

            var camera = new CameraModel2D(components, 1920, 1080, 5.0f, Vector2.Zero);

            camera.SetWorldRotationRadiansClockwiseFromPositiveY((float)Math.PI * 0.5f);

            var up = camera.GetWorldUp();

            Assert.Equal(1.0f, up.X, 4);
            Assert.Equal(0.0f, up.Y, 4);
        }

        [Fact]
        public void Camera2DTest_UpdatingCamera_EnsureCorrectNumberOfCountsToUpdatingMatrixBuffers()
        {
            var components = Substitute.For<ISystemComponents>();

            var camera = new CameraModel2D(components, 1920, 1080, 5.0f, Vector2.Zero);

            components.ClearReceivedCalls();

            camera.SetWorldFocus(Vector2.Zero); //x1
            camera.SetWorldZoom(2.0f); //x1
            camera.SetWorldRotationUsingUpVector(Vector2.UnitY); //x1
            camera.SetWorldRotationRadiansClockwiseFromPositiveY(45.0f); //x1
            camera.SetWorldFocusAndZoom(Vector2.Zero, 2.0f); //x1
            camera.SetWorldFocusZoomAndRotationRadiansAngleClockwiseFromPositiveY(Vector2.Zero, 2.0f, 45.0f); //x3
            camera.SetWorldFocusZoomAndRotationUsingUpVector(Vector2.Zero, 2.0f, Vector2.UnitX); //x3

            components.Device.Received(11).UpdateBuffer(Arg.Any<DeviceBuffer>(), Arg.Any<uint>(), ref Arg.Any<Matrix4x4>());
        }
    }
}