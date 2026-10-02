# yak2D 

The yak2D framework enables the creation of interactive **cross platform** desktop 2D graphics applications.

It offers simple 2D polygon (coloured, textured and dual textured) drawing functions, in addition to flexible render path creation making use of shader effects (both inbuilt and user defined).

Quickly create 2D games and prototypes that run on all major desktop operating systems. Graphics, Input, Windowing and Application Lifecycle are all provided for and managed by the framework (*just add sound*). Avoid the bloat of large game engines (or make life harder for yourself - whatever your opinion). No GUI, **do it all in code..** :)

yak2D is a .NET 10 library, built upon [NeoVeldrid](https://www.nuget.org/packages/NeoVeldrid) (a maintained fork of the [Veldrid](https://github.com/mellinoe/veldrid) cross-platform, graphics API agnostic rendering library). Application windowing and input are handled by SDL2, which is bundled with the NuGet package - there are no native libraries to install.

**Supported Desktop Platforms: Windows, Linux and macOS** (x64 and arm64)

**Supported Graphics APIs: Direct3D 11, Vulkan, OpenGL** *(Metal support is currently disabled; macOS runs on OpenGL, or Vulkan via MoltenVK)*

![](logo.png) 

[![NuGet](https://img.shields.io/nuget/v/yak2d.svg)](https://www.nuget.org/packages/Yak2D/)

## Documentation and Samples

[Documentation](https://alzpatz.github.io/yak2d-docs/) - including a step by step [Getting Started](https://alzpatz.github.io/yak2d-docs/articles/gettingstarted.html) tutorial

[Demo Samples](https://github.com/AlzPatz/yak2d-samples)

## Key Features
* Customisable Rendering Pipeline
    * Create and use Textures and Render Targets (surfaces) as  inputs and outputs wherever desired in rendering pipeline
    * Arrange render stages in any order to create desired effects
    * Supports common image file formats for Texture loading
* 2D Drawing
    * Draw custom polygons from vertices or regular shapes (quads, n-sided shapes) by creating request queues
    * Helpers provided for generating vertices of common shapes, including a fluent interface for reusing and iterating drawing objects
    * Fill with solid colour, single or dual texturing
    * Draw / transform into world space or screen space (based on interchangable cameras), split into layers and set depths
    * Reuse queues or re-create each frame
    * Queues auto-sorted and batched 
* Bitmap Font Rendering support
    * Will parse user .fnt files
* Use of Cameras (2D and 3D) and Viewports allow easy rendering of the same draw queue or surface from different perspectives, on differet parts of a render surface
    * Simplifies split screen views
* Shader Effects
    * Blur, Bloom, Colourize, Grayscale, Negative, Add Opacity, Mix Textures, Basic Copy between surfaces
    * Pixellate, Static, Edge Detection, Old-Movie Reel, CRT monitor 
    * Height Map Distortion (such as shock waves)
    * Render surfaces to 3D meshes (Phong lighting model with up to 8 lights)
    * Easily Create Custom Shader stages, or stages with full exposure to NeoVeldrid objects (including compute shaders)
* GPU to CPU surface copies (read back rendered pixel data)
* Input
    * Exposes keyboard, mouse and gamepad input via an abstraction over NeoVeldrid / SDL2

## Installation

yak2D requires the [.NET 10 SDK](https://dotnet.microsoft.com/download) (or later) on Windows, Linux or macOS.

Add yak2D to your project from NuGet:

```shell
dotnet add package Yak2D
```

or search for `Yak2D` in the NuGet package manager of Visual Studio or Rider.

## Usage

1. Create a new console application and add yak2D:
    ```shell
    dotnet new console -n MyApplicationName
    cd MyApplicationName
    dotnet add package Yak2D
    ```

2. Create a class implementing the `IApplication` interface. Its methods are called by the framework in this order:
    * **OnStartup()**
      - Runs before Configure(). Add non-yak2D related code if desired to run before other methods are called
    * **Configure()**
      - Return a `StartupConfig` containing the configuration properties for the framework (window resolution, graphics API, update timestep type, etc). `StartupConfig.Default(...)` provides sensible defaults
    * **CreateResources()**
      - Runs once on start up, where the user can create required framework resources (surfaces, render stages, fonts, etc). Can make sense to manually call when graphics device is lost, or any time resources are lost
    * **ProcessMessage()**
      - Runs before an Update() iteration. Allows user to process important messages. Top Tip: GraphicsDeviceRecreated will require the recreation of all framework objects (surfaces, render stages, fonts, etc) - common usage is to call CreateResources(). SwapChainFramebufferReCreated will invalidate any current references held to the framebuffer
    * **Update()**
      - Runs once per simulation update. Return false to exit the application
    * **PreDrawing()**
      - Runs before Drawing(). A good time to set effect configurations and clear draw queues
    * **Drawing()**
      - User should build DrawStage and DistortionStage draw request queues here
    * **Rendering()**
      - Build the rendering pipeline by queuing up render stages
    * **Shutdown()**
      - Runs once as application, well, shuts down ...

3. In `Program.cs`, pass your IApplication object to the static method **Launcher.Run()**:
    ```csharp
    using Yak2D;

    Launcher.Run(new MyApplication());
    ```

4. Build and Run!
    ```shell
    dotnet run
    ```

## What's next?

Please see the [Documentation](https://alzpatz.github.io/yak2d-docs/) for more detailed usage information, tutorials and API description and also check out the [Demo Samples](https://github.com/AlzPatz/yak2d-samples)!

## Package Sources, Versions and CI

yak2D is continually released from the master branch.

A github workflow / action is used to manage the ci build and publishing.

When the version number (3 digit form, i.e 1.2.3) is updated in version.json, a new **Yak2D** RELEASE package is published on NuGet.org:

[![NuGet](https://img.shields.io/nuget/v/yak2d.svg)](https://www.nuget.org/packages/Yak2D/)

**Yak2D-dev** Development / DEBUG packages are available from [MyGet](https://www.myget.org/feed/Packages/yak2d-dev). These are published whenever the master branch has a code commit in /src (including when the version.json number is not updated) - the package name is suffixed with -dev and the version number includes a 4th component (and potential commit id based string) autogenerated using nerdbank.gitversioning during the ci build and package steps. A package is also pushed to [MyGet](https://www.myget.org/feed/Packages/yak2d-dev) whenever a commit is made on a non-master branch that contains "push-pack-dev" in the commit string. 

The solution is built on Windows, Linux and macOS for every push. Tests run on Windows and Linux, and must pass for a package to be published to any source.

Pushing to master also triggers a rebuild of the [Documentation](https://alzpatz.github.io/yak2d-docs/), whose API reference is generated from the source code comments.

Finally, for a push to any branch, or pull request to master, packages are uploaded as downloadable build artifacts on github.

## Contribute

If anyone would like to contribute, fix bugs, modify, whatever ... they are more than welcome to submit a pull request

## Bugs and Issues

Let me know - I will try to address any that come up!

## Credits

[Veldrid](https://github.com/mellinoe/veldrid) is awesome, and Eric is a great person. Thanks also to the maintainers of [NeoVeldrid](https://www.nuget.org/packages/NeoVeldrid) for keeping it going, and to [Silk.NET](https://github.com/dotnet/Silk.NET) and [SDL](https://www.libsdl.org/).

## License

MIT
