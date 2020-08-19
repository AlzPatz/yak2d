using System.Collections.Generic;
using System.IO;

namespace Yak2D.Font
{
    public interface IFontLoader
    {
        List<string> FindDotFntFileNamePartialMatchesFromEmbeddedResource(bool isFrameworkInternal, string providedAssetPathNameWithoutAssemblyOrExtension);
        List<string> FindDotFntFileNamePartialMatchesFromFileResource(string providedAssetPathNameWithoutAssemblyOrExtension);
        List<string> ReadStreamToStringList(Stream stream);
        Stream LoadEmbeddedStream(bool isFrameworkInternal, string resourcePathName);
        Stream LoadFileStream(string resourcePathName);
        CandidateSubFontDesc TryToLoadSubFontDescription(string fontBaseFolderWithoutAssemblyDoesNotEndInDivisor, bool isFrameworkInternal, AssetSourceEnum assetType, List<string> fntFileLines);
        FontModel GenerateFontFromDescriptionInfo(CandidateFontDesc fontDescription);
    }
}
