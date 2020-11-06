using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class CoordinateTransforms : ICoordinateTransforms
    {
        private ISystemComponents _systemComponents;
        private ICameraManager _cameraManager;
        private IViewportManager _viewportManager;

        public CoordinateTransforms(ISystemComponents systemComponents,
                                    ICameraManager cameraManager,
                                    IViewportManager viewportManager)
        {
            _systemComponents = systemComponents;
            _cameraManager = cameraManager;
            _viewportManager = viewportManager;
        }

        public Vector2 ScreenFromWorld(Vector2 position, ICamera2D camera)
        {
            if (camera == null)
            {
                return Vector2.Zero;
            }

            return ScreenFromWorld(position, camera.Id);
        }

        public Vector2 ScreenFromWorld(Vector2 position, ulong camera)
        {
            var cam = _cameraManager.RetrieveCameraModel2D(camera);

            if (cam == null)
            {
                return Vector2.Zero;
            }

            return ScreenFromWorld(position, cam);
        }

        private Vector2 ScreenFromWorld(Vector2 position, ICameraModel2D cam)
        {
            var worldFocusToPoint = position - cam.GetWorldFocus();
            var rotatedToScreenOrientation = Utility.Geometry.RotateVectorClockwise(worldFocusToPoint, -cam.GetWorldClockwiseRotationRadsFromPositiveY());
            return rotatedToScreenOrientation * cam.GetWorldZoom();
        }

        public Vector2 WorldFromScreen(Vector2 position, ICamera2D camera)
        {
            if (camera == null)
            {
                return Vector2.Zero;
            }

            return WorldFromScreen(position, camera.Id);
        }

        public Vector2 WorldFromScreen(Vector2 position, ulong camera)
        {
            var cam = _cameraManager.RetrieveCameraModel2D(camera);

            if (cam == null)
            {
                return Vector2.Zero;
            }

            return WorldFromScreen(position, cam);
        }

        private Vector2 WorldFromScreen(Vector2 position, ICameraModel2D cam)
        {
            var up = cam.GetWorldUp();
            var right = new Vector2(up.Y, -up.X);
            position /= cam.GetWorldZoom();
            var shiftFromFocus = (position.X * right) + (position.Y * up);
            return cam.GetWorldFocus() + shiftFromFocus;
        }

        public TransformResult ScreenFromWindow(Vector2 position, ICamera2D camera, IViewport viewport = null)
        {
            if (camera == null)
            {
                return new TransformResult
                {
                    Contained = false,
                    Position = Vector2.Zero
                };
            }

            return ScreenFromWindow(position, camera.Id, viewport == null ? null : (ulong?)viewport.Id);
        }

        public TransformResult ScreenFromWindow(Vector2 position, ulong camera, ulong? viewport = null)
        {
            var cam = _cameraManager.RetrieveCameraModel2D(camera);

            if (cam == null)
            {
                return new TransformResult
                {
                    Contained = false,
                    Position = Vector2.Zero
                };
            }

            IViewportModel vport = null;

            if (viewport != null)
            {
                vport = _viewportManager.RetrieveViewportModel((ulong)viewport);
            }

            return ScreenFromWindow(position, cam, vport);
        }

        private TransformResult ScreenFromWindow(Vector2 position, ICameraModel2D cam, IViewportModel viewport)
        {
            var viewPositionOriginTopLeft = position;
            var viewWidth = (float)_systemComponents.Window.Width;
            var viewHeight = (float)_systemComponents.Window.Height;

            if (viewport != null)
            {
                viewWidth = viewport.Width;
                viewHeight = viewport.Height;
                viewPositionOriginTopLeft -= new Vector2(viewport.MinX, viewport.MinY);
            }

            var viewPositionTransformed = new Vector2(viewPositionOriginTopLeft.X, viewHeight - viewPositionOriginTopLeft.Y);
            viewPositionTransformed -= 0.5f * new Vector2(viewWidth, viewHeight);

            var camVirtualResolution = cam.GetVirtualResolution();
            var resolutionScalar = new Vector2(camVirtualResolution.X / viewWidth, camVirtualResolution.Y / viewHeight);

            var screenPosition = viewPositionTransformed * resolutionScalar;

            var withinView = screenPosition.X >= -0.5f * camVirtualResolution.X &&
                             screenPosition.X <= 0.5f * camVirtualResolution.X && //Perhaps < preferred
                             screenPosition.Y >= -0.5f * camVirtualResolution.Y &&
                             screenPosition.Y <= 0.5f * camVirtualResolution.Y; //Perhaps < preferred

            return new TransformResult
            {
                Contained = withinView,
                Position = screenPosition
            };
        }

        public TransformResult WindowFromScreen(Vector2 position, ICamera2D camera, IViewport viewport = null)
        {
            if (camera == null)
            {
                return new TransformResult
                {
                    Contained = false,
                    Position = Vector2.Zero
                };
            }

            return WindowFromScreen(position, camera.Id, viewport == null ? null : (ulong?)viewport.Id);
        }

        public TransformResult WindowFromScreen(Vector2 position, ulong camera, ulong? viewport = null)
        {
            var cam = _cameraManager.RetrieveCameraModel2D(camera);

            if (cam == null)
            {
                return new TransformResult
                {
                    Contained = false,
                    Position = Vector2.Zero
                };
            }

            IViewportModel vport = null;

            if (viewport != null)
            {
                vport = _viewportManager.RetrieveViewportModel((ulong)viewport);
            }

            return WindowFromScreen(position, cam, vport);
        }

        private TransformResult WindowFromScreen(Vector2 position, ICameraModel2D cam, IViewportModel viewport)
        {
            var windowResolution = new Vector2((float)_systemComponents.Window.Width, (float)_systemComponents.Window.Height);
            var screenSpaceCentreView = Vector2.Zero;
            var viewWidth = windowResolution.X;
            var viewHeight = windowResolution.Y;

            if (viewport != null)
            {
                var windowSpaceCentreView = new Vector2((float)(viewport.MinX + (0.5f * viewport.Width)), (float)(viewport.MinY + (0.5f * viewport.Height)));
                screenSpaceCentreView = new Vector2(windowSpaceCentreView.X, windowResolution.Y - windowSpaceCentreView.Y) - (0.5f * windowResolution);
                viewWidth = viewport.Width;
                viewHeight = viewport.Height;
            }

            var camVirtualResolution = cam.GetVirtualResolution();
            var resolutionScalar = new Vector2(viewWidth / camVirtualResolution.X, viewHeight / camVirtualResolution.Y);

            var screenSpacePosition = screenSpaceCentreView + (position * resolutionScalar);

            var windowPosition = (0.5f * windowResolution) + new Vector2(screenSpacePosition.X, -screenSpacePosition.Y);

            var withinView = windowPosition.X >= 0.0f &&
                           windowPosition.X <= windowResolution.X && //Perhaps < preferred
                           windowPosition.Y >= 0.0f &&
                           windowPosition.Y <= windowResolution.Y; //Perhaps < preferred

            return new TransformResult
            {
                Contained = withinView,
                Position = windowPosition
            };
        }

        public TransformResult WindowFromWorld(Vector2 position, ICamera2D camera, IViewport viewport = null)
        {
            if (camera == null)
            {
                return new TransformResult
                {
                    Contained = false,
                    Position = Vector2.Zero
                };
            }

            return WindowFromWorld(position, camera.Id, viewport == null ? null : (ulong?)viewport.Id);
        }

        public TransformResult WindowFromWorld(Vector2 position, ulong camera, ulong? viewport = null)
        {
            var cam = _cameraManager.RetrieveCameraModel2D(camera);

            if (cam == null)
            {
                return new TransformResult
                {
                    Contained = false,
                    Position = Vector2.Zero
                };
            }

            IViewportModel vport = null;

            if (viewport != null)
            {
                vport = _viewportManager.RetrieveViewportModel((ulong)viewport);
            }

            return WindowFromWorld(position, cam, vport);
        }

        private TransformResult WindowFromWorld(Vector2 position, ICameraModel2D camera, IViewportModel viewport)
        {
            var screen = ScreenFromWorld(position, camera);
            return WindowFromScreen(screen, camera, viewport);
        }

        public TransformResult WorldFromWindow(Vector2 position, ICamera2D camera, IViewport viewport = null)
        {
            if (camera == null)
            {
                return new TransformResult
                {
                    Contained = false,
                    Position = Vector2.Zero
                };
            }

            return WorldFromWindow(position, camera.Id, viewport == null ? null : (ulong?)viewport.Id);
        }

        public TransformResult WorldFromWindow(Vector2 position, ulong camera, ulong? viewport = null)
        {
            var cam = _cameraManager.RetrieveCameraModel2D(camera);

            if (cam == null)
            {
                return new TransformResult
                {
                    Contained = false,
                    Position = Vector2.Zero
                };
            }

            IViewportModel vport = null;

            if (viewport != null)
            {
                vport = _viewportManager.RetrieveViewportModel((ulong)viewport);
            }

            return WorldFromWindow(position, cam, vport);
        }

        private TransformResult WorldFromWindow(Vector2 position, ICameraModel2D camera, IViewportModel viewport)
        {
            var screen = ScreenFromWindow(position, camera, viewport);
            var world = screen;
            world.Position = WorldFromScreen(screen.Position, camera);
            return world;
        }
    }
}