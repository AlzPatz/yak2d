using System.Collections.Generic;
using System.Numerics;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class CameraManager : ICameraManager
    {
        public int Count2D { get { return _camera2DCollection.Count; } }
        public int Count3D { get { return _camera3DCollection.Count; } }

        private readonly IIdGenerator _idGenerator;
        private readonly ICameraFactory _cameraFactory;

        private ISimpleCollection<ICameraModel2D> _camera2DCollection;
        private ISimpleCollection<ICameraModel3D> _camera3DCollection;

        private List<ulong> _camerasToDestroy;

        public CameraManager(ICameraFactory cameraFactory,
                                IIdGenerator idGenerator,
                                ISimpleCollectionFactory collectionFactory)

        {
            _idGenerator = idGenerator;
            _cameraFactory = cameraFactory;

            _camera2DCollection = collectionFactory.Create<ICameraModel2D>(32);
            _camera3DCollection = collectionFactory.Create<ICameraModel3D>(16);

            _camerasToDestroy = new List<ulong>();
        }

        public ICameraModel2D RetrieveCameraModel2D(ulong key) => _camera2DCollection.Retrieve(key);
        public ICameraModel3D RetrieveCameraModel3D(ulong key) => _camera3DCollection.Retrieve(key);

        public ICamera2D CreateCamera2D(uint virtualResolutionWidth = 960,
                                        uint virtualResolutionHeight = 540,
                                        float zoom = 1.0f)
        {
            var id = _idGenerator.New();

            var model = _cameraFactory.CreateCamera2D(virtualResolutionWidth,
                                                      virtualResolutionHeight,
                                                      zoom);

            var userReference = new Camera2D(id);

            return _camera2DCollection.Add(id, model) ? userReference : null;
        }

        public ICamera3D CreateCamera3D(Vector3 position,
                                        Vector3 lookAt,
                                        Vector3 up,
                                        float fieldOfViewDegress = 60.0f,
                                        float aspectRation = 16.0f / 9.0f,
                                        float nearPlane = 0.0001f,
                                        float farPlane = 1000.0f)
        {
            var id = _idGenerator.New();

            var model = _cameraFactory.CreateCamera3D(position,
                                                      lookAt,
                                                      up,
                                                      fieldOfViewDegress,
                                                      aspectRation,
                                                      nearPlane,
                                                      farPlane);

            var userReference = new Camera3D(id);

            return _camera3DCollection.Add(id, model) ? userReference : null;
        }

        public void DestroyCamera(ulong key)
        {
            _camerasToDestroy.Add(key);
        }

        public void DestroyAllCameras()
        {
            DestroyAllCameras2D();
            DestroyAllCameras3D();
        }

        public void DestroyAllCameras2D()
        {
            var ids2D = _camera2DCollection.ReturnAllIds();
            ids2D.ForEach(id =>
            {
                _camerasToDestroy.Add(id);
            });
        }

        public void DestroyAllCameras3D()
        {
            var ids3D = _camera3DCollection.ReturnAllIds();
            ids3D.ForEach(id =>
            {
                _camerasToDestroy.Add(id);
            });
        }

        public void ProcessPendingDestruction()
        {
            _camerasToDestroy.ForEach(id =>
            {

                if (_camera2DCollection.Contains(id))
                {
                    var cam2d = _camera2DCollection.Retrieve(id);
                    cam2d.Destroy();
                    _camera2DCollection.Remove(id);
                    return;
                }

                if (_camera3DCollection.Contains(id))
                {
                    var cam3d = _camera3DCollection.Retrieve(id);
                    cam3d.Destroy();
                    _camera3DCollection.Remove(id);
                }
            });

            _camerasToDestroy.Clear();
        }

        public void Shutdown()
        {
            DestroyAllCameras();
            ProcessPendingDestruction();
        }
    }
}