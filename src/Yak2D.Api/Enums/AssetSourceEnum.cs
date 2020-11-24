namespace Yak2D
{
    /// <summary>
    /// Assets (Textures, Fonts) can be either embedded as resources or included in the application output directory
    /// Embedded resources must be part of the assembly that provides the implementation of IApplication to the framework launcher
    ///
    /// Below note that angle brackets have been replaced with () to avoid xml formatting complaints
    /// An example of .csproj content that would embed in the application assembly all files in the Textures folder (found in the .csproj root folder):
    /// (ItemGroup) (EmbeddedResource Include="Textures**"/) (/ItemGroup)
    ///
    /// An example of .csproj content that would include in the output directory all of the files in the Fonts folder (found in the .csproj root folder):
    /// (ItemGroup) (Content Include="$(MSBuildThisFileDirectory)Fonts\**") (CopyToOutputDirectory)PreserveNewest(/CopyToOutputDirectory)(Link)Fonts\%(RecursiveDir)\%(FileName)%(Extension)(/Link)(/Content)(/ItemGroup)
    /// The () around MSBuildThisFileDirectory are correct, the rest should be replaced with angled brackets
    ///
    /// Currently it is not possible to provide an absolute file path to the application to load an asset from any location
    /// </summary>
    public enum AssetSourceEnum
    {
        /// <summary>
        /// Represents a file on disk in the application directory
        /// </summary>
        File,

        /// <summary>
        /// Represents an asset that has been embedded in the application assembly
        /// </summary>
        Embedded,
    }
}