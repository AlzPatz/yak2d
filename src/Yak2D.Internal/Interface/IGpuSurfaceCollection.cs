namespace Yak2D.Internal
{
    public interface IGpuSurfaceCollection
    {
        int CountAll();
        int CountOfType(GpuSurfaceType type);
        bool Add(ulong id, GpuSurface surface);
        bool Remove(ulong id);
        void RemoveAllOfType(GpuSurfaceType type);
        void RemoveAll(bool resourcesAlreadyDestroyed);
        GpuSurface Retrieve(ulong id);
    }
}