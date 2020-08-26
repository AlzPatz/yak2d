# yak2D

The yak2D framework enables the creation of interactive **cross platform** desktop 2D graphics applications.

It offers simple 2D polygon and ‘sprite’ drawing functions, in addition to flexible render path creation making use of shader effects (both inbuilt and user defined).

Quickly create 2D games and prototypes that run on all major desktop operating systems. Graphics, Input, Windowing and Application Lifecycle are all provided for and managed by the framework (*just add sound*). Avoid the bloat of large game engines. **Do it all in code..** no GUIs.

yak2D is structured as a collection of .NET standard 2.0 class libraries, built upon the [Veldrid](https://github.com/mellinoe/veldrid) cross-platform API agnostic rendering library. Application windowing is handled by SDL2 via [Veldrid](https://github.com/mellinoe/veldrid).

**Supported Desktop Platforms: Windows, Linux and OSX**

**Supported Graphics APIs: Direct3D 11, Vulkan, Open GL, Metal**

## Documentation

[yak2D documentation](https://alzpatz.github.io/yak2d-docs/)

## Installation 

Add yak2D to your project via your Nuget Package Manager:
* Visual Studio:
    ![Search via Visual Studio](.github/nuget_vs.png?raw=true)
* .NET core command line
    ```shell
    dotnet add package yak2D --version *
    ```
    Replace `*` with desired version number 

yak2D, like Veldrid, uses the standard .NET core tooling. [Install the tools](https://www.microsoft.com/net/download/core) and build normally ('dotnet build')

## Usage 

1. Create a new .NET core console application
    * Visual Studio:
    Create a new console application using the wizard
    * .NET core command line 
    ```shell
    dotnet new console -n MyApplicationName
    ```
2. Add yak2D to your project via the Nuget package manager
3. Create a class overriding the IApplication interface 
    Implement the interface methods:
        * Method
        * Method
        * Method
4. In Program.cs, or whether appropriate, pass your IApplication object to the static method Launcher.Run()
5. Build and Run!
    * .NET core command line
    ```shell
    dotnet run
    ```

## What's next?

Please see the [Documentation](https://alzpatz.github.io/yak2d-docs/) for additional information and also check out the [Demo Samples](https://github.com/AlzPatz/yak2d-samples)!

## Package Sources, Versions and CI

yak2D is continually released from the master branch.

A github workflow / action is used to manage the ci build and publishing.

When the version number (3 digit form, 1.2.3 to 1.2.4 for example) is updated in version.json, a new RELEASE package is published on NuGet.org:

[![NuGet](https://img.shields.io/nuget/v/yak2d.svg)](https://www.nuget.org/packages/Yak2D/)

Development / DEBUG packages are avaliable from [MyGet](https://www.https://www.myget.org/feed/Packages/yak2d-dev). These are published whenever the master branch has a commit - the package name is suffixed with -dev and the version number includes a 4th component (and potential commit id based string) autogenerated using nerdbank.gitversioning during the ci build and package steps. A package is also pushed to [MyGet](https://www.https://www.myget.org/feed/Packages/yak2d-dev) whenever a commit is made on a non-master branch that contains "push-pack-dev" in the commit string. 

Note - all test must pass for a package to be published to any source

Finally, for a push to any branch, or pull request to master, packages are uploaded as downloadable build artifacts on github.
