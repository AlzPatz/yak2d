# yak2D

The yak2D framework enables the creation of interactive **cross platform** desktop 2D graphics applications.

It offers simple 2D polygon and ‘sprite’ drawing functions, in addition to flexible render path creation making use of shader effects (both inbuilt and user defined).

Quickly create 2D games and prototypes that run on all major desktop operating systems. Graphics, Input, Windowing and Application Lifecycle are all provided for and managed by the framework (*just add sound*). Avoid the bloat of large game engines. **Do it all in code..** no GUIs.

yak2D is structured as a collection of .NET standard 2.0 class libraries, built upon the Veldrid cross-platform API agnostic rendering library. Application windowing is handled by SDL2 via the Veldrid.

[Veldrid](https://github.com/mellinoe/veldrid)

### Supported Desktop Platforms
* Windows 
* Linux 
* OSX 

### Supported Graphics APIs
* Direct3D 11 
* Vulkan 
* Open GL 
* Metal 

## Documentation

[yak2D documentation](https://alzpatz.github.io/yak2d-docs/)

## Installation 

Add yak2D to your project via your Nuget Package Manager 

Visual Studio GUI:

![Search via Visual Studio](.github/nuget_vs.png?raw=true)

.NET core command line

'''shell
dotnet add package yak2D --version *
'''

yak2D, like Veldrid, uses the standard .NET core tooling. [Install the tools](https://www.microsoft.com/net/download/core) and build normally ('dotnet build')

## Usage 

After adding yak2D to your project, create your own class overriding the IApplication interface.

Pass your IApplication object to Launcher.Run()

Build and Run!

## What's next?

Please see the [Documentation](https://alzpatz.github.io/yak2d-docs/) for additional information and also check out the [Demo Samples](https://github.com/AlzPatz/yak2d-samples)!
