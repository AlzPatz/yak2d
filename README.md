# yak2D 

The yak2D framework enables the creation of interactive **cross platform** desktop 2D graphics applications.

It offers simple 2D polygon and ‘sprite’ drawing functions, in addition to flexible render path creation making use of shader effects (both inbuilt and user defined).

Quickly create 2D games and prototypes that run on all major desktop operating systems. Graphics, Input, Windowing and Application Lifecycle are all provided for and managed by the framework (*just add sound*). Avoid the bloat of large game engines (or make life harder for yourself - whatever your opinion). No GUI, **do it all in code..** :)

yak2D is structured as a collection of .NET standard 2.0 class libraries, built upon the [Veldrid](https://github.com/mellinoe/veldrid) cross-platform API agnostic rendering library. Application windowing is handled by SDL2 via [Veldrid](https://github.com/mellinoe/veldrid).

**Supported Desktop Platforms: Windows, Linux and OSX**

**Supported Graphics APIs: Direct3D 11, Vulkan, Open GL, Metal**

![](logo.png) 

[![NuGet](https://img.shields.io/nuget/v/yak2d.svg)](https://www.nuget.org/packages/Yak2D/)

## Documentation and Samples

[Documentation](https://alzpatz.github.io/yak2d-docs/) 

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
    * Easily Create Custom Shader stages, or stages with full exposure to Veldrid objects
* Input
    * Exposes keyboard, mouse and gamepad input via an abstraction over Veldrid / SDL2

## Installation 

Add yak2D to your project via your Nuget Package Manager:
* Visual Studio:
    ![Search via Visual Studio](.github/nuget_vs.png?raw=true)
* .NET core command line
    ```shell
    dotnet add package yak2D --version *
    ```
    Replace `*` with desired version number or leave it as is to download the latest version

yak2D, like Veldrid, uses the standard .NET core tooling. [Install the tools](https://www.microsoft.com/net/download/core) and build normally ('dotnet build')

## Usage 

1. Create a new .NET core console application
    * Visual Studio:
    Create a new console application using the wizard
    * .NET core command line 
    ```shell
    dotnet new console -n MyApplicationName
    ```
    
2. Add yak2D to your project via the Nuget package manager (see installation above)

3. Create a class overriding the IApplication interface. Implement the interface methods: 
    * **OnStartup()**
      - Runs before Configure(). Add non-yak2D related code if desired to run before other methods are called
    * **Configure()**
      - Return an object containing the configuration properties for the framework (window resolution, update timestep type, etc)
    * **ProcessMessage()**
      - Runs before an Update() iteration. Allows user to process important messages. Top Tip: GraphicsDeviceRecreated will require the recreation of all framework objects (surfaces, render stages, fonts, etc). Common usage is for the user to make it to call CreateResources() and SwapChainFramebufferReCreated will invalid any current references held to the framebuffer
    * **CreateResources()**
      - Runs once on start up, where the user can create required framework resources (surfaces, render stages, fonts, etc). Can make sense to manually call when graphics device is lost, or any time resources are lost
    * **Update()**
      - Runs once per simulation update
    * **PreDrawing()**
      - Runs before Drawing(). A good time to set effect configurations and clear draw queues  
    * **Drawing()**
      - User should build DrawStage and DistortionStage draw request queues here
    * **Rending()**
      - Build the rendering pipeline by queuing up draw stages
    * **Shutdown()**
      - Runs once as application, well, shuts down ...
    
4. In Program.cs, or whether appropriate, pass your IApplication object to the static method **Launcher.Run()**

5. Build and Run!
    * .NET core command line
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

**Yak2D-dev** Development / DEBUG packages are avaliable from [MyGet](https://www.myget.org/feed/Packages/yak2d-dev). These are published whenever the master branch has a code commit in /src (including when the version.json number is not updated) - the package name is suffixed with -dev and the version number includes a 4th component (and potential commit id based string) autogenerated using nerdbank.gitversioning during the ci build and package steps. A package is also pushed to [MyGet](https://www.myget.org/feed/Packages/yak2d-dev) whenever a commit is made on a non-master branch that contains "push-pack-dev" in the commit string. 

Note - all test must pass for a package to be published to any source

Finally, for a push to any branch, or pull request to master, packages are uploaded as downloadable build artifacts on github.

## Contribute

If anyone would like to contribute, fix bugs, modify, whatever ... they are more than welcome to submit a pull request

## Bugs and Issues

Let me know - I will try to address any that come up!

## Credits

[Veldrid](https://github.com/mellinoe/veldrid) is awesome, and Eric is a great person.

## License

MIT and Apache 2.0. If anyone knows of a fast, stable sorting algorithm (merge or Tim sort, or equivalent) under MIT license that can replace the use of HPCsharp - I would be keen to hear about it (so can drop Apache)
