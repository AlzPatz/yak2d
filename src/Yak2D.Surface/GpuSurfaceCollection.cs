using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Surface
{
    public class GpuSurfaceCollection : IGpuSurfaceCollection
    {
        private readonly IFrameworkMessenger _frameworkMessenger;

        private Dictionary<ulong, GpuSurface> _surfaces;

        public GpuSurfaceCollection(IFrameworkMessenger frameworkMessenger)
        {
            _frameworkMessenger = frameworkMessenger;
            _surfaces = new Dictionary<ulong, GpuSurface>();
        }

        public bool Add(ulong id, GpuSurface surface)
        {
            if (surface == null)
            {
                _frameworkMessenger.Report("GpuSurfaceCollection: Will not add null surface to collection");
                return false;
            }

            if (_surfaces.ContainsKey(id))
            {
                _frameworkMessenger.Report("GpuSurfaceCollection: Unable to add surface, ulong exists in collection");
                return false;
            }

            _surfaces.Add(id, surface);
            return true;
        }

        public int CountAll()
        {
            return _surfaces.Count;
        }

        public int CountOfType(GpuSurfaceType type)
        {
            var count = 0;
            foreach (var surface in _surfaces.Values)
            {
                if (surface.Type.HasFlag(type))
                {
                    count++;
                }
            }
            return count;
        }

        public bool Remove(ulong id)
        {
            if (!_surfaces.ContainsKey(id))
            {
                _frameworkMessenger.Report("GpuSurfaceCollection: Unable to remove surface, ulong DOES NOT exist in collection");
                return false;
            }

            var surface = _surfaces[id];

            DisposeOfASurface(surface);

            _surfaces.Remove(id);
            return true;
        }

        private void DisposeOfASurface(GpuSurface surface)
        {
            surface.Framebuffer?.Dispose();
            surface.ResourceSet_TexWrap?.Dispose();
            surface.ResourceSet_TexMirror?.Dispose();
            surface.Texture?.Dispose();
            surface.TextureView?.Dispose();
        }

        public void RemoveAllOfType(GpuSurfaceType type)
        {
            var listToRemove = new List<ulong>();
            foreach (var surface in _surfaces)
            {
                if (surface.Value.Type.HasFlag(type))
                {
                    DisposeOfASurface(surface.Value);
                    listToRemove.Add(surface.Key);
                }
            }
            listToRemove.ForEach(x => _surfaces.Remove(x));
        }

        public void RemoveAll(bool resourcesAlreadyDestroyed)
        {
            if (!resourcesAlreadyDestroyed)
            {
                foreach (var surface in _surfaces.Values)
                {
                    DisposeOfASurface(surface);
                }
            }
            _surfaces.Clear();
        }

        public GpuSurface Retrieve(ulong id)
        {
            GpuSurface surface;
            if (_surfaces.TryGetValue(id, out surface)) //TRYGET used as potential optimisation to check CONTAINS and then DICTIONARY LOOK UP (although surely compiler optimises for that anyway...)
            {
                return surface;
            }
            else
            {
                _frameworkMessenger.Report("GpuSurfaceCollection: Unable to retrieve surface as ulong does not exist in collection");
                return null;
            }
        }
    }
}
