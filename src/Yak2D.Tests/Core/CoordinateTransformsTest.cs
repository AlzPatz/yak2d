using NSubstitute;
using System;
using System.Numerics;
using Xunit;
using Yak2D.Core;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class CoordinateTransformsTest
    {
        private ICameras _cameras;
        private ViewportManager _viewportManager;
        private CoordinateTransforms _transforms;
        private Vector2 _windowResolution;

        public CoordinateTransformsTest()
        {
            _windowResolution = new Vector2(960.0f, 540.0f);
            var window = Substitute.For<IWindow>();
            window.Width.Returns((uint)_windowResolution.X);
            window.Height.Returns((uint)_windowResolution.Y);
            var components = Substitute.For<ISystemComponents>();
            components.Window.Returns(window);
            var messenger = Substitute.For<IFrameworkMessenger>();
            var cameraFactory = new CameraFactory(components);
            var idGenerator = new IdGenerator(messenger);
            var collectionFactory = new SimpleDictionaryCollectionFactory(messenger);
            var cameraManager = new CameraManager(cameraFactory, idGenerator, collectionFactory);
            var viewportFactory = new ViewportFactory();
            _viewportManager = new ViewportManager(viewportFactory, idGenerator, collectionFactory);
            _transforms = new CoordinateTransforms(components, cameraManager, _viewportManager);
            _cameras = new Cameras(cameraManager);
        }

        [Fact]
        public void ScreenFromWorld_TestAtOriginUnitZoom()
        {
            var camera = _cameras.CreateCamera2D();

            _cameras.SetCamera2DFocusAndZoom(camera, Vector2.Zero, 1.0f);

            Assert.Equal(Vector2.Zero, _transforms.ScreenFromWorld(Vector2.Zero, camera));
        }

        [Fact]
        public void ScreenFromWorld_TestAwayFromOriginUnitZoom()
        {
            var camera = _cameras.CreateCamera2D();

            _cameras.SetCamera2DFocusAndZoom(camera, new Vector2(50.0f, 100.0f), 1.0f);

            var positionToTest = new Vector2(-150.0f, -200.0f);

            Assert.Equal(new Vector2(-200.0f, -300.0f), _transforms.ScreenFromWorld(positionToTest, camera));
        }

        [Fact]
        public void ScreenFromWorld_TestAwayFromOriginZoomed()
        {
            var camera = _cameras.CreateCamera2D();

            _cameras.SetCamera2DFocusAndZoom(camera, new Vector2(50.0f, 100.0f), 2.0f);

            var positionToTest = new Vector2(-50.0f, -50.0f);

            Assert.Equal(new Vector2(-200.0f, -300.0f), _transforms.ScreenFromWorld(positionToTest, camera));
        }

        [Fact]
        public void ScreenFromWorld_TestAwayFromOriginZoomedRotatedAngle()
        {
            var camera = _cameras.CreateCamera2D();

            _cameras.SetCamera2DFocusZoomAndRotation(camera, new Vector2(50.0f, 100.0f), 2.0f, 0.5f * (float)Math.PI);

            var positionToTest = new Vector2(-100.0f, 200.0f);

            Assert.Equal(new Vector2(-200.0f, -300.0f), _transforms.ScreenFromWorld(positionToTest, camera));
        }

        [Fact]
        public void ScreenFromWorld_TestAwayFromOriginZoomedRotatedByVector()
        {
            var camera = _cameras.CreateCamera2D();

            _cameras.SetCamera2DFocusZoomAndRotation(camera, new Vector2(50.0f, 100.0f), 2.0f, Vector2.UnitX);

            var positionToTest = new Vector2(-100.0f, 200.0f);

            Assert.Equal(new Vector2(-200.0f, -300.0f), _transforms.ScreenFromWorld(positionToTest, camera));
        }

        [Fact]
        public void WorldFromScreen_TestAtOriginUnitZoom()
        {
            var camera = _cameras.CreateCamera2D();

            _cameras.SetCamera2DFocusAndZoom(camera, Vector2.Zero, 1.0f);

            Assert.Equal(Vector2.Zero, _transforms.WorldFromScreen(Vector2.Zero, camera));
        }

        [Fact]
        public void WorldFromScreen_TestAwayFromOriginUnitZoom()
        {
            var camera = _cameras.CreateCamera2D();

            _cameras.SetCamera2DFocusAndZoom(camera, new Vector2(50.0f, 100.0f), 1.0f);

            var positionToTest = new Vector2(-200.0f, -300.0f);

            Assert.Equal(new Vector2(-150.0f, -200.0f), _transforms.WorldFromScreen(positionToTest, camera));
        }

        [Fact]
        public void WorldFromScreen_TestAwayFromOriginZoomed()
        {
            var camera = _cameras.CreateCamera2D();

            _cameras.SetCamera2DFocusAndZoom(camera, new Vector2(50.0f, 100.0f), 2.0f);

            var positionToTest = new Vector2(-200.0f, -300.0f);

            Assert.Equal(new Vector2(-50.0f, -50.0f), _transforms.WorldFromScreen(positionToTest, camera));
        }

        [Fact]
        public void WorldFromScreen_TestAwayFromOriginZoomedRotatedAngle()
        {
            var camera = _cameras.CreateCamera2D();

            _cameras.SetCamera2DFocusZoomAndRotation(camera, new Vector2(50.0f, 100.0f), 2.0f, 0.5f * (float)Math.PI);

            var positionToTest = new Vector2(-200.0f, -300.0f);

            Assert.Equal(new Vector2(-100.0f, 200.0f), _transforms.WorldFromScreen(positionToTest, camera));
        }

        [Fact]
        public void WorldFromScreen_TestAwayFromOriginZoomedRotatedByVector()
        {
            var camera = _cameras.CreateCamera2D();

            _cameras.SetCamera2DFocusZoomAndRotation(camera, new Vector2(50.0f, 100.0f), 2.0f, Vector2.UnitX);

            var positionToTest = new Vector2(-200.0f, -300.0f);

            Assert.Equal(new Vector2(-100.0f, 200.0f), _transforms.WorldFromScreen(positionToTest, camera));
        }

        [Fact]
        public void ScreenFromWindow_CentreScreenNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = 0.5f * _windowResolution;

            var screenPositionExpected = Vector2.Zero;

            var result = _transforms.ScreenFromWindow(windowPosition, camera, null);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void ScreenFromWindow_CentreScreenNoViewportContainedDoubledVirtualResolution()
        {
            var camera = _cameras.CreateCamera2D(2 * (uint)_windowResolution.X, 2 * (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = 0.5f * _windowResolution;

            var screenPositionExpected = Vector2.Zero;

            var result = _transforms.ScreenFromWindow(windowPosition, camera, null);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void ScreenFromWindow_TopLeftQuadrantPointOnScreenNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = 0.25f * _windowResolution;

            var screenPositionExpected = new Vector2(-0.25f * _windowResolution.X, 0.25f * _windowResolution.Y);

            var result = _transforms.ScreenFromWindow(windowPosition, camera, null);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void ScreenFromWindow_BottomRightQuadrantPointOnScreenNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = 0.75f * _windowResolution;

            var screenPositionExpected = new Vector2(0.25f * _windowResolution.X, -0.25f * _windowResolution.Y);

            var result = _transforms.ScreenFromWindow(windowPosition, camera, null);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void ScreenFromWindow_BottomRightQuadrantPointOnScreenNoViewportContainedDoubleResolution()
        {
            var camera = _cameras.CreateCamera2D(2 * (uint)_windowResolution.X, 2 * (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = 0.75f * _windowResolution;

            var screenPositionExpected = new Vector2(0.5f * _windowResolution.X, -0.5f * _windowResolution.Y);

            var result = _transforms.ScreenFromWindow(windowPosition, camera, null);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void ScreenFromWindow_CentreScreenCentredHalfSizedViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            var viewport = _viewportManager.CreateViewport((uint)(0.25f * _windowResolution.X),
                                                           (uint)(0.25f * _windowResolution.Y),
                                                           (uint)(0.5f * _windowResolution.X),
                                                           (uint)(0.5f * _windowResolution.Y));

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = 0.5f * _windowResolution;

            var screenPositionExpected = Vector2.Zero;

            var result = _transforms.ScreenFromWindow(windowPosition, camera, viewport);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void ScreenFromWindow_TopLeftCornerOfViewportOnScreenCentredHalfSizedViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            var viewport = _viewportManager.CreateViewport((uint)(0.25f * _windowResolution.X),
                                                           (uint)(0.25f * _windowResolution.Y),
                                                           (uint)(0.5f * _windowResolution.X),
                                                           (uint)(0.5f * _windowResolution.Y));

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = 0.25f * _windowResolution;

            var screenPositionExpected = new Vector2(-0.5f * _windowResolution.X, 0.5f * _windowResolution.Y);

            var result = _transforms.ScreenFromWindow(windowPosition, camera, viewport);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void ScreenFromWindow_TopLeftScreenQuadrantOutsideOfViewportCentredHalfSizedViewportNotContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            var viewport = _viewportManager.CreateViewport((uint)(0.25f * _windowResolution.X),
                                                           (uint)(0.25f * _windowResolution.Y),
                                                           (uint)(0.5f * _windowResolution.X),
                                                           (uint)(0.5f * _windowResolution.Y));

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = 0.1f * _windowResolution;

            var screenPositionExpected = new Vector2(-0.8f * _windowResolution.X, 0.8f * _windowResolution.Y);

            var result = _transforms.ScreenFromWindow(windowPosition, camera, viewport);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.False(result.Contained);
        }

        [Fact]
        public void ScreenFromWindow_CentredPointInsideUnCentredViewportWithNonStandardResolution()
        {
            var camera = _cameras.CreateCamera2D(75, 60);

            var viewport = _viewportManager.CreateViewport(200,
                                                           200,
                                                           200,
                                                           100);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = new Vector2(300.0f, 250.0f);

            var screenPositionExpected = Vector2.Zero;

            var result = _transforms.ScreenFromWindow(windowPosition, camera, viewport);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void ScreenFromWindow_TopLeftQuadrantPointInsideUnCentredViewportWithNonStandardResolution()
        {
            var camera = _cameras.CreateCamera2D(80, 60);

            var viewport = _viewportManager.CreateViewport(200,
                                                           200,
                                                           200,
                                                           100);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = new Vector2(250.0f, 225.0f);

            var screenPositionExpected = new Vector2(-20.0f, 15.0f);

            var result = _transforms.ScreenFromWindow(windowPosition, camera, viewport);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void ScreenFromWindow_OutsideBottomRightUnCentredViewportWithNonStandardResolution()
        {
            var camera = _cameras.CreateCamera2D(80, 60);

            var viewport = _viewportManager.CreateViewport(200,
                                                           200,
                                                           200,
                                                           100);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var windowPosition = new Vector2(500.0f, 400.0f);

            var screenPositionExpected = new Vector2(80.0f, -90.0f);

            var result = _transforms.ScreenFromWindow(windowPosition, camera, viewport);

            Assert.Equal(screenPositionExpected.X, result.Position.X, 4);
            Assert.Equal(screenPositionExpected.Y, result.Position.Y, 4);
            Assert.False(result.Contained);
        }

        [Fact]
        public void WindowFromScreen_CentreOfScreenNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var screenPosition = Vector2.Zero;

            var windowPositionExpected = 0.5f * _windowResolution;

            var result = _transforms.WindowFromScreen(screenPosition, camera, null);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WindowFromScreen_TopLeftQuadrantNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var screenPosition = new Vector2(-0.25f * _windowResolution.X, 0.25f * _windowResolution.Y);

            var windowPositionExpected = 0.25f * _windowResolution;

            var result = _transforms.WindowFromScreen(screenPosition, camera, null);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WindowFromScreen_BottomRightQuadrantNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var screenPosition = new Vector2(0.25f * _windowResolution.X, -0.25f * _windowResolution.Y);

            var windowPositionExpected = 0.75f * _windowResolution;

            var result = _transforms.WindowFromScreen(screenPosition, camera, null);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WindowFromScreen_BottomRightQuadrantNoViewportUnContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var screenPosition = new Vector2(_windowResolution.X, -_windowResolution.Y);

            var windowPositionExpected = 1.5f * _windowResolution;

            var result = _transforms.WindowFromScreen(screenPosition, camera, null);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.False(result.Contained);
        }

        [Fact]
        public void WindowFromScreen_CentreOfViewportContained()
        {
            var camera = _cameras.CreateCamera2D(200, 100, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var viewport = _viewportManager.CreateViewport(300,
                                                        200,
                                                        400,
                                                        200);

            var screenPosition = Vector2.Zero;

            var windowPositionExpected = new Vector2(500.0f, 300.0f);

            var result = _transforms.WindowFromScreen(screenPosition, camera, viewport);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WindowFromScreen_PointInViewportContained()
        {
            var camera = _cameras.CreateCamera2D(200, 100, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var viewport = _viewportManager.CreateViewport(300,
                                                        200,
                                                        400,
                                                        200);

            var screenPosition = new Vector2(-50.0f, 25.0f);

            var windowPositionExpected = new Vector2(400.0f, 250.0f);

            var result = _transforms.WindowFromScreen(screenPosition, camera, viewport);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WindowFromScreen_PointInViewportNotContainedInWindow()
        {
            var camera = _cameras.CreateCamera2D(200, 100, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var viewport = _viewportManager.CreateViewport(300,
                                                           200,
                                                           400,
                                                           200);

            var screenPosition = new Vector2(250.0f, -150.0f);

            var windowPositionExpected = new Vector2(1000.0f, 600.0f);

            var result = _transforms.WindowFromScreen(screenPosition, camera, viewport);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.False(result.Contained);
        }

        [Fact]
        public void WindowFromWorld_OriginNoShiftNoZoomNoRotationNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var worldPosition = new Vector2(0.0f, 0.0f);

            var windowPositionExpected = 0.5f * _windowResolution;

            var result = _transforms.WindowFromWorld(worldPosition, camera, null);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WindowFromWorld_WorldPointNoZoomNoRotationNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var worldPosition = new Vector2(300.0f, 200.0f);

            var windowPositionExpected = (0.5f * _windowResolution) + new Vector2(300.0f, -200.0f);

            var result = _transforms.WindowFromWorld(worldPosition, camera, null);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WindowFromWorld_WorldPointZoomedNoRotationNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 0.5f, 0.0f);

            var worldPosition = new Vector2(300.0f, 200.0f);

            var windowPositionExpected = (0.5f * _windowResolution) + new Vector2(150.0f, -100.0f);

            var result = _transforms.WindowFromWorld(worldPosition, camera, null);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WindowFromWorld_WorldPointZoomedRotatedNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 0.5f, 0.5f * (float)Math.PI);

            var worldPosition = new Vector2(300.0f, 200.0f);

            var windowPositionExpected = (0.5f * _windowResolution) + new Vector2(-100.0f, -150.0f);

            var result = _transforms.WindowFromWorld(worldPosition, camera, null);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WindowFromWorld_ShiftedZoomedRotatedViewportWithDifferingResolutionContained()
        {
            var camera = _cameras.CreateCamera2D((uint)200, (uint)100, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, new Vector2(24.0f, 15.0f), 2.0f, 0.5f * (float)Math.PI);

            var viewport = _viewportManager.CreateViewport(300,
                                                         200,
                                                         400,
                                                         200);

            var worldPosition = new Vector2(30.0f, 10.0f);

            var windowPositionExpected = new Vector2(500.0f, 300.0f) + new Vector2(20.0f, -24.0f);

            var result = _transforms.WindowFromWorld(worldPosition, camera, viewport);

            Assert.Equal(windowPositionExpected.X, result.Position.X, 4);
            Assert.Equal(windowPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WorldFromWindow_OriginNoShiftNoZoomNoRotationNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var worldPositionExpected = new Vector2(0.0f, 0.0f);

            var windowPosition = 0.5f * _windowResolution;

            var result = _transforms.WorldFromWindow(windowPosition, camera, null);

            Assert.Equal(worldPositionExpected.X, result.Position.X, 4);
            Assert.Equal(worldPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WorldFromWindow_WorldPointNoZoomNoRotationNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 1.0f, 0.0f);

            var worldPositionExpected = new Vector2(300.0f, 200.0f);

            var windowPosition = (0.5f * _windowResolution) + new Vector2(300.0f, -200.0f);

            var result = _transforms.WorldFromWindow(windowPosition, camera, null);

            Assert.Equal(worldPositionExpected.X, result.Position.X, 4);
            Assert.Equal(worldPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WorldFromWindow_WorldPointZoomedNoRotationNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 0.5f, 0.0f);

            var worldPositionExpected = new Vector2(300.0f, 200.0f);

            var windowPosition = (0.5f * _windowResolution) + new Vector2(150.0f, -100.0f);

            var result = _transforms.WorldFromWindow(windowPosition, camera, null);

            Assert.Equal(worldPositionExpected.X, result.Position.X, 4);
            Assert.Equal(worldPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WorldFromWindow_WorldPointZoomedRotatedNoViewportContained()
        {
            var camera = _cameras.CreateCamera2D((uint)_windowResolution.X, (uint)_windowResolution.Y, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, Vector2.Zero, 0.5f, 0.5f * (float)Math.PI);

            var worldPositionExpected = new Vector2(300.0f, 200.0f);

            var windowPosition = (0.5f * _windowResolution) + new Vector2(-100.0f, -150.0f);

            var result = _transforms.WorldFromWindow(windowPosition, camera, null);

            Assert.Equal(worldPositionExpected.X, result.Position.X, 4);
            Assert.Equal(worldPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }

        [Fact]
        public void WorldFromWindow_ShiftedZoomedRotatedViewportWithDifferingResolutionContained()
        {
            var camera = _cameras.CreateCamera2D((uint)200, (uint)100, 1.0f);

            _cameras.SetCamera2DFocusZoomAndRotation(camera, new Vector2(24.0f, 15.0f), 2.0f, 0.5f * (float)Math.PI);

            var viewport = _viewportManager.CreateViewport(300,
                                                         200,
                                                         400,
                                                         200);

            var worldPositionExpected = new Vector2(30.0f, 10.0f);

            var windowPosition = new Vector2(500.0f, 300.0f) + new Vector2(20.0f, -24.0f);

            var result = _transforms.WorldFromWindow(windowPosition, camera, viewport);

            Assert.Equal(worldPositionExpected.X, result.Position.X, 4);
            Assert.Equal(worldPositionExpected.Y, result.Position.Y, 4);
            Assert.True(result.Contained);
        }
    }
}