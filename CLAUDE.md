# CLAUDE.md

Guidance for Claude Code (and smith runs) working in this repo.

yak2D is a cross-platform (Windows/Linux/macOS) 2D graphics/game framework built on NeoVeldrid
(D3D11/Vulkan/OpenGL/Metal) + SDL2, published to NuGet as `Yak2D`. It's a .NET (net10.0) solution:
`src/Yak2D.sln`.

## Commits

Commit as the user only: never add `Co-Authored-By: Claude` trailers, "Generated with Claude Code"
lines or any other AI attribution to commit messages or PR descriptions.

## Commands

Run from `src/`:

```shell
dotnet restore
dotnet build -c Release --no-restore
dotnet test
```

- **Single test**: `dotnet test --filter "FullyQualifiedName~StagesTest"`. Tests are xunit.v3
  `[Fact]`s, so filter by class or method name.
- **Tests need a real GPU device** (`Yak2D.Tests/ManualFakes/FakeGraphicsDevice.cs` creates one).
  They run on Windows (D3D11) and Linux (Vulkan, needs SDL2; works headless on Mesa's lavapipe
  software driver). CI (`.github/workflows/test_pack_and_push.yml`) runs `dotnet test` on
  `windows-latest` and `ubuntu-latest` (installing `libsdl2-2.0-0`, `libvulkan1`,
  `mesa-vulkan-drivers` first) and skips it on macOS. Build and restore run on all three OSes.

## Versioning and releases

- The NuGet version comes from `version.json` (`Major.Minor.Patch`) and Nerdbank.GitVersioning,
  in CI only. Don't add NBGV as a project package, and don't hand-edit version numbers in
  `.csproj` files.
- **Bump `version.json` in every change to master that alters what ships**: anything under `src/`
  except `Yak2D.Tests`, including XML doc comments and dependency versions. Use patch for fixes,
  docs and internal changes, minor for new public API, and major for breaking API changes.
  NuGet.org only gets a new package when the version changes. Otherwise the push is a 409
  Conflict skipped by `--skip-duplicate`, and only the MyGet `-dev` package updates.
- Test-, CI-, README- and CLAUDE.md-only changes need no bump.
- A push to master also rebuilds the docs site (yak2d-docs-content) via `repository_dispatch`.

## Who depends on this

Other repos use yak2d. They are normally checked out beside this one, but each builds on its own:

- **yak2d-samples** builds against this repo's source when it is beside it, else the `Yak2D`
  package.
- **yak2d-docs-content**, **gas2d** and **sis-escape-run** use the published `Yak2D` package only.

A public API change can therefore break the samples straight away, and the others at their next
package update.

## Architecture

`src/Yak2D.sln` is layered as `Api` (public contracts) → feature projects (implementation) →
`Yak2D` (composition root):

- **`Yak2D.Api`**: the entire public surface consumers depend on. Interfaces
  (`Interface/{Services,RenderStages,Items,Utilities}`), enums and structs; no logic, just contracts.
- **`Yak2D.Internal`**: shared internal interfaces, objects and utilities (e.g.
  `IApplicationAssembly`, `IApplicationMessenger`) used across projects to avoid circular
  references. Not exposed to consumers.
- **`Yak2D.Core`**: app lifecycle. The update/draw loop (`Core.cs`), timing, FPS, SDL2 window and
  event plumbing, shutdown.
- **`Yak2D.Graphics`**: the rendering engine. Pipelines, cameras, viewports, GPU data structs and
  the render-stage system (`Stages/*`, see below).
- **`Yak2D.Surface`**: GPU surface (render target / texture) management, system textures, and
  image loading (StbImageSharp).
- **`Yak2D.Font`**: font loading/parsing (`.fnt` files) and rendering.
- **`Yak2D.Input`**: keyboard, mouse and gamepad input; wraps Veldrid/SDL2.
- **`Yak2D.Utility`**: general-purpose helpers shared across projects.
- **`Yak2D`**: the composition root and NuGet package entry point. `Launcher.cs` builds a
  SimpleInjector `Container`, registers ~90 services (one
  `container.Register<Interface, Impl>(Lifestyle.Singleton)` per service across every project
  above), calls `container.Verify()`, resolves `ICore` and runs it.
- **`Yak2D.Tests`**: xunit.v3 + NSubstitute (mocking). Mirrors the source layout (`Core/`, `Font/`,
  `Graphics/`, `Surface/`, `Utility/`), with hand-written test doubles in `ManualFakes/`.

**Api vs implementation pattern**: each feature project has a `User/` folder with the concrete
class implementing a public `Yak2D.Api` interface, backed by internal services. For example
`IBackend` (`Yak2D.Api`) is implemented by `Backend` (`Yak2D.Core/User/Backend.cs`),
`IStages`→`Stages` (`Yak2D.Graphics/User/Stages.cs`) and `ISurfaces`→`Surfaces`
(`Yak2D.Surface/User/Surfaces.cs`). To add a public capability, add the interface to `Yak2D.Api`,
implement it in the owning project's `User/` folder, and wire it into `Launcher.cs`.

**Consumer application lifecycle**: a consumer implements `IApplication` and passes it to
`Launcher.Run()`. Callback order: `OnStartup()` → `Configure()` (returns window/timestep config) →
`CreateResources()` (create surfaces/stages/fonts) → per frame: `ProcessMessage()` → `Update()` →
`PreDrawing()` → `Drawing()` (build draw queues) → `Rendering()` (build the render pipeline from
stages) → `Shutdown()` on exit. `GraphicsDeviceRecreated`/`SwapChainFramebufferReCreated` messages
mean the consumer must recreate framework resources.

**Render Stage abstraction**
(`Yak2D.Graphics/Stages/{Bloom,Blur,Colour,Copy,CustomShader,CustomVeldrid,Distortion,DownSampling,Drawing,GpuToCpuSurfaceCopy,Mesh,Mix,Style}/`):
each effect follows a 3-class pattern. A thin `XStage` (just an `Id`), an `XStageModel` holding
per-instance config/state (`Update`, `SendToRenderStage`, `DestroyResources`), and an
`XStageRenderer` doing the actual Veldrid GPU work. A `RenderStageManager`/`RenderStageModelFactory`
creates and tracks models by id; a visitor (`IRenderStageVisitor`) dispatches queued render commands
to the correct renderer. To add a new stage: add the Stage/Model/Renderer trio, register the
renderer in `Launcher.cs`, expose it via `IStages`, and add a demo in yak2d-samples.

Shaders are precompiled per backend under `Yak2D.Graphics/Shaders/{Direct3D,Metal,OpenGL,Vulkan}/`.
