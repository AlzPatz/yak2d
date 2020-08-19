using Yak2D.Internal;

namespace Yak2D.Font
{
    public interface IFontCollection
    {
        int Count { get; }
        bool Add(ulong id, IFontModel font);
        bool Destroy(ulong id);
        void DestroyAll(bool resourcesAlreadyDestroyed);
        IFontModel Retrieve(ulong id);
    }
}
