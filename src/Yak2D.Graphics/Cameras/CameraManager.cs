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

        public CameraManager(ICameraFactory cameraFactory,
                                IIdGenerator idGenerator,
                                ISimpleCollectionFactory collectionFactory)
                                
        {
            _idGenerator = idGenerator;
            _cameraFactory = cameraFactory;

            _camera2DCollection = collectionFactory.Create<ICameraModel2D>(32);
            _camera3DCollection = collectionFactory.Create<ICameraModel3D>(16);
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
            if (_camera2DCollection.Contains(key))
            {
                var cam2d = _camera2DCollection.Retrieve(key);
                cam2d.Destroy();
                _camera2DCollection.Remove(key);
                return;
            }

            if (_camera3DCollection.Contains(key))
            {
                var cam3d = _camera3DCollection.Retrieve(key);
                cam3d.Destroy();
                _camera3DCollection.Remove(key);
            }
        }

        public void DestroyAllCameras()
        {
            DestroyAllCameras2D();
            DestroyAllCameras3D();
        }

        public void Shutdown()
        {
            //No difference between this and DestroyAllCameras
            DestroyAllCameras();
        }

        public void DestroyAllCameras2D()
        {
            foreach (var cam in _camera2DCollection.Iterate())
            {
                cam.Destroy();
            }

            _camera2DCollection.RemoveAll();
        }

        public void DestroyAllCameras3D()
        {
            foreach (var cam in _camera3DCollection.Iterate())
            {
                cam.Destroy();
            }

            _camera3DCollection.RemoveAll();
        }
    }
}