using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public interface IFontCollection
    {
        int Count { get; }
        bool Add(ulong id, IFontModel font);
        void Destroy(ulong id, bool resourcesAlreadyDestroyed);
        IFontModel Retrieve(ulong id);
        List<ulong> ReturnAllIds();
    }
}
