using System.IO;
using System.Collections.Generic;
using System.Linq;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public class FontManager : IFontManager
    {
        private const string _systemFontFolder = "SystemFonts"; //Embedded Resources. Use '.' to Delimit folders if modify

        public IFontModel SystemFont { get; private set; }

        public int UserFontCount { get { return _userFontCollection.Count; } }

        private readonly StartupConfig _startUpProperties;
        private readonly IIdGenerator _idGenerator;
        private readonly IFontLoader _fontLoader;
        private readonly IFontCollection _userFontCollection;



        public FontManager(IIdGenerator idGenerator,
                                      IStartupPropertiesCache startUpPropertiesCache,
                                      IFontCollection fontCollection,
                                      IFontLoader fontLoader)
        {
            _idGenerator = idGenerator;
            _fontLoader = fontLoader;
            _userFontCollection = fontCollection;
            _startUpProperties = startUpPropertiesCache.User;

            LoadSystemFonts();
        }

        public void Shutdown()
        {
            DestroyAllUserFonts(true);
        }

        public void ReInitialise()
        {
            DestroyAllUserFonts(true);
            LoadSystemFonts();
        }

        private void LoadSystemFonts()
        {
            SystemFont = LoadFont(true, "notosans", AssetSourceEnum.Embedded);
        }

        public IFont LoadUserFont(string fontPathWithoutExtension, AssetSourceEnum assetType)
        {
            //Guards and exceptions for null or white space strings already exist at the IFont Level
            if (string.IsNullOrWhiteSpace(fontPathWithoutExtension) || fontPathWithoutExtension.Trim().Any(char.IsWhiteSpace))
            {
                throw new Yak2DException(string.Concat("Unable to load user font as path is null or contains whitespace"));
            }

            fontPathWithoutExtension = fontPathWithoutExtension.Trim();

            var font = LoadFont(false, fontPathWithoutExtension, assetType);

            var id = _idGenerator.New();

            return _userFontCollection.Add(id, font) ? new FontReference(id) : null;
        }

        private IFontModel LoadFont(bool isFrameworkInternal, string pathToFontNameNoExtension, AssetSourceEnum assetType)
        {
            //There is a .fnt file for each size of a font. The .fnt file contains information on spacing and importantly,
            //what the texture files are called for that font size. When loading a font, we attempt to load each font size
            //as an additional subfont. Hence here, we seach for font name partial/leading matches. i.e. 
            //"notosans" would find font sizes such as "notosans_22.fnt", "notosans_24.fnt", etc. We then try and load each 
            //of those font files as a sub font. The seach path may include additional folders, such as "/mono/notosans" etc
            //We always match the entire search string with the leading part of the font name

            var pathWithoutAssemblyOrExtension = string.Concat(isFrameworkInternal ? _systemFontFolder : _startUpProperties.FontFolder,
                                                                        isFrameworkInternal ? "." : "/",
                                                                        pathToFontNameNoExtension);

            pathWithoutAssemblyOrExtension = assetType == AssetSourceEnum.Embedded ? pathWithoutAssemblyOrExtension.Replace("/", ".") :
                                                                                     pathWithoutAssemblyOrExtension.Replace(".", "/");

            var fontBaseFolderWithoutAssemblyDoesNotEndInDivisor =
                assetType == AssetSourceEnum.Embedded ? pathWithoutAssemblyOrExtension.Substring(0, pathWithoutAssemblyOrExtension.LastIndexOf(".")) :
                                                        pathWithoutAssemblyOrExtension.Substring(0, pathWithoutAssemblyOrExtension.LastIndexOf("/"));

            var candidateSubFontDotFntResourceFileNames = new List<string> { };

            switch (assetType)
            {
                case AssetSourceEnum.Embedded:
                    candidateSubFontDotFntResourceFileNames = _fontLoader.FindDotFntFileNamePartialMatchesFromEmbeddedResource(isFrameworkInternal, pathWithoutAssemblyOrExtension);
                    break;
                case AssetSourceEnum.File:
                    candidateSubFontDotFntResourceFileNames = _fontLoader.FindDotFntFileNamePartialMatchesFromFileResource(pathWithoutAssemblyOrExtension);
                    break;
            }

            if (candidateSubFontDotFntResourceFileNames.Count == 0)
            {
                throw new Yak2DException(string.Concat("Unable to complete Load Font request, no potential .fnt resources were found: ",
                                                    pathWithoutAssemblyOrExtension, " , ", assetType.ToString(), ",",
                                                    " Framework Internal?== ", isFrameworkInternal));

            }

            candidateSubFontDotFntResourceFileNames.ForEach(x =>
            {
                if (x.Any(char.IsWhiteSpace))
                {
                    throw new Yak2DException(string.Concat("Unable to complete Load Font request, resource names must not contain whitespace: ", x));
                };
            });

            //These LINQs could be collapsed, but split out for now to aid reading
            var candidateSubFontDotFntFileStreams = candidateSubFontDotFntResourceFileNames.Select(resourcePathName =>
            {
                Stream stream = null;

                //Switch rather than one line conditional in case future requires other content types
                switch (assetType)
                {
                    case AssetSourceEnum.Embedded:
                        stream = _fontLoader.LoadEmbeddedStream(isFrameworkInternal, resourcePathName);
                        break;
                    case AssetSourceEnum.File:
                        stream = _fontLoader.LoadFileStream(resourcePathName);
                        break;
                }

                return stream;
            }).ToList();

            var streamStrings = candidateSubFontDotFntFileStreams.Select(stream =>
            {
                return _fontLoader.ReadStreamToStringList(stream);
            }).Where(lines => lines.Count > 0).ToList();

            var candidateFontDesc = new CandidateFontDesc
            {
                Name = pathToFontNameNoExtension,
                CandidateSubFonts = streamStrings.Select(lines => _fontLoader.TryToLoadSubFontDescription(
                fontBaseFolderWithoutAssemblyDoesNotEndInDivisor,
                isFrameworkInternal,
                assetType,
                lines)).Where(desc => desc != null).ToList()
            };

            if (candidateFontDesc.CandidateSubFonts.Count == 0)
            {
                throw new Yak2DException(string.Concat(string.Concat("Unable to complete font request, failed to load any sub font descriptors: ",
                                                    pathToFontNameNoExtension, " , ", assetType.ToString(), ",",
                                                    " Framework Internal?== ", isFrameworkInternal)));
            }

            return _fontLoader.GenerateFontFromDescriptionInfo(candidateFontDesc);
        }

        public IFontModel RetrieveFont(ulong id)
        {
            return _userFontCollection.Retrieve(id);
        }

        public void DestroyFont(ulong id)
        {
            _userFontCollection.Destroy(id);
        }

        public void DestroyAllUserFonts(bool resoucesDestroyedAlready)
        {
            _userFontCollection.DestroyAll(resoucesDestroyedAlready);
        }
    }
}