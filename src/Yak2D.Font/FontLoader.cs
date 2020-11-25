using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public class FontLoader : IFontLoader
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;
        private readonly ISubFontGenerator _subFontGenerator;
        private readonly IFileSystem _fileSystem;
        private readonly IAssembly _applicationAssembly;
        private readonly IAssembly _fontsAssembly;

        private readonly StartupConfig _startUpProperties;

        public FontLoader(  IApplicationAssembly applicationAssembly,
                            IFontsAssembly fontsAssembly, 
                            IFrameworkMessenger frameworkMessenger,
                            IGpuSurfaceManager gpuSurfaceManager,
                            IStartupPropertiesCache startUpPropertiesCache,
                            ISubFontGenerator subFontGenerator,
                            IFileSystem fileSystem)

        {
            _applicationAssembly = applicationAssembly;
            _fontsAssembly = fontsAssembly;
            _frameworkMessenger = frameworkMessenger;
            _gpuSurfaceManager = gpuSurfaceManager;
            _subFontGenerator = subFontGenerator;
            _fileSystem = fileSystem;

            _startUpProperties = startUpPropertiesCache.User;
        }

        public List<string> FindDotFntFileNamePartialMatchesFromEmbeddedResource(bool isFrameworkInternal, string providedAssetPathNameWithoutAssemblyOrExtension)
        {
            //Append correct assembly name
            var fullSearchPath = string.Concat(isFrameworkInternal ? _fontsAssembly.Name : _applicationAssembly.Name, ".", providedAssetPathNameWithoutAssemblyOrExtension);

            var resourceNames = isFrameworkInternal ? _fontsAssembly.GetManifestResourceNames() : _applicationAssembly.GetManifestResourceNames();

            //Find all .fnt files that start with full search path
            var matches = resourceNames.Where(x => x.Contains(fullSearchPath)).Where(x => x.Contains(".fnt")).ToList();

            return matches;
        }

        public List<string> FindDotFntFileNamePartialMatchesFromFileResource(string providedAssetPathNameWithoutAssemblyOrExtension)
        {
            var files = _fileSystem.GetFilesInDirectory(_startUpProperties.FontFolder, "*.*", SearchOption.AllDirectories);

            var matches = files.Where(x => x.Contains(providedAssetPathNameWithoutAssemblyOrExtension)).Where(x => x.Contains(".fnt")).ToList();

            return matches;
        }

        public Stream LoadEmbeddedStream(bool isFrameworkInternal, string resourcePathName)
        {
            var assembly = isFrameworkInternal ? _fontsAssembly : _applicationAssembly;

            //It is bad to assume, but I know these were previously found so no skipping check if exists
            var stream = assembly.GetManifestResourceStream(resourcePathName);

            return stream;
        }

        public Stream LoadFileStream(string resourcePathName)
        {
            //It is bad to assume, but I know these were previously found so no skipping check if exists
            FileStream stream = _fileSystem.OpenRead(resourcePathName);

            return stream;
        }

        public List<string> ReadStreamToStringList(Stream stream)
        {
            if (stream == null)
            {
                return new List<string>();
            }

            var reader = new StreamReader(stream);
            var lines = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            stream?.Dispose();

            return lines;
        }

        public CandidateSubFontDesc TryToLoadSubFontDescription(string fontBaseFolderWithoutAssemblyDoesNotEndInDivisor,
                                                                bool isFrameworkInternal,
                                                                AssetSourceEnum assetType,
                                                                ImageFormat imageFormat,
                                                                List<string> fntFileLines)
        {
            var resourceFolder = fontBaseFolderWithoutAssemblyDoesNotEndInDivisor;

            var extractedTexturePathsWithoutBasePath = _subFontGenerator.ExtractPngFilePathsFromDotFntLines(fntFileLines);

            if (extractedTexturePathsWithoutBasePath.Count == 0)
            {
                return null; //Fail Message already displayed in earlier method
            }

            var texturePathsWithoutBasePathsWithoutFileExtension = extractedTexturePathsWithoutBasePath.Select(x =>
            {
                if (!x.EndsWith(".png"))
                {
                    _frameworkMessenger.Report("Warning -> expected texture path name does not end with .png");
                }

                return x.Remove(x.Length - 4, 4);
            }).ToList();

            var textures = new List<ITexture>();

            texturePathsWithoutBasePathsWithoutFileExtension.ForEach(texturePath =>
            {
                ITexture texture = null;
                var texturePathWithoutExtension = string.Concat(resourceFolder, assetType == AssetSourceEnum.Embedded ? "." : "/", texturePath);

                switch (assetType)
                {
                    case AssetSourceEnum.Embedded:
                        texture = _gpuSurfaceManager.CreateFontTextureFromEmbeddedResource(isFrameworkInternal,
                                                                                           texturePathWithoutExtension,
                                                                                           imageFormat,
                                                                                           SamplerType.Anisotropic);
                        break;
                    case AssetSourceEnum.File:
                        texture = _gpuSurfaceManager.CreateFontTextureFromFile(texturePathWithoutExtension, imageFormat, SamplerType.Anisotropic);
                        break;
                }
                if (texture != null)
                {
                    textures.Add(texture);
                }
            });

            if (textures.Count == 0)
            {
                _frameworkMessenger.Report(string.Concat("Texture loads for font failed: ",
                                    resourceFolder, ",",
                                    " Framework Internal?== ", isFrameworkInternal));
            }

            return new CandidateSubFontDesc
            {
                DotFntLines = fntFileLines,
                TexturePaths = texturePathsWithoutBasePathsWithoutFileExtension,
                Textures = textures
            };
        }

        public FontModel GenerateFontFromDescriptionInfo(CandidateFontDesc fontDescription)
        {
            if (fontDescription == null)
            {
                _frameworkMessenger.Report("Null font description object passed to font loader");
                return null;
            }

            var subFontDescs = fontDescription.CandidateSubFonts;

            var subFonts = new List<SubFont>();

            subFontDescs.ForEach(desc =>
            {
                var subFont = _subFontGenerator.Generate(desc);
                if (subFont == null)
                {
                    _frameworkMessenger.Report(string.Concat("Failed to create subfont within: ", fontDescription.Name));
                }
                else
                {
                    subFonts.Add(subFont);
                }
            });

            return new FontModel(subFonts);
        }
    }
}