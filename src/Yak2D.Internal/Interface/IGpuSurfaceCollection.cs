using System.Collections.Generic;

namespace Yak2D.Internal
{
    public interface IGpuSurfaceCollection
    {
        int CountAll();
        int CountOfType(GpuSurfaceType type);
        bool Add(ulong id, GpuSurface surface);
        bool Remove(ulong id);
        List<ulong> ReturnAllOfType(GpuSurfaceType type);
        void RemoveAll(bool resourcesAlreadyDestroyed);
        GpuSurface Retrieve(ulong id);
    }
}