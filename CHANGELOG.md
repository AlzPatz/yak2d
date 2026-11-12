# Changelog

## 1.3.0

- **Raw-pixel textures are now mirrored on OpenGL, like loaded image files.** `ISurfaces.CreateRgbaFromData` and
  `ISurfaces.CreateFloat32FromData` now mirror rows on OpenGL / OpenGLES (using the same rule as file loads) and leave
  them unchanged on Vulkan, Direct3D11 and Metal. Index 0 is the top-left pixel and row 0 is the top of the image on
  every backend, so the same array draws the same way up everywhere. **Remove any hand-mirroring of pixel rows you did
  for OpenGL**, or the texture will now draw upside down there. `ISurfaces.LoadTextureColourData` now also returns
  `TextureData.Pixels` with row 0 at the top on every backend (it was previously mirrored on OpenGL), so passing its
  output to `CreateRgbaFromData` is mirrored once.
- **Vulkan DualTextured Texture1 orientation fix.** The second texture of dual-textured draws was sampled upside down on
  Vulkan; it now matches the other backends.
- **Draw layers of 10 and above now draw.** The drawing stage's layer depth scaling now grows with the number of layers
  in use, so items on layer 10 or higher are no longer lost.
- **New `YAK2D_TEXTURE_ROWS_BOTTOM_UP` define for SPIR-V custom shaders.** Custom shader stages created with
  `useSpirvCompile: true` get `#define YAK2D_TEXTURE_ROWS_BOTTOM_UP 1` on OpenGL (texture rows stored bottom-up) or
  `0` on Vulkan, Direct3D11 and Metal, inserted directly after the `#version` line. `FTex` is already correct on every
  backend; use the define to correct texture coordinates the shader computes itself (world-space tiling, atlas cells):

  ```glsl
  #if YAK2D_TEXTURE_ROWS_BOTTOM_UP
      uv.y = 1.0 - uv.y;
  #endif
  ```

  Shaders that don't use it compile and behave as before. Precompiled per-backend shaders don't receive the define; a
  `Shaders/OpenGL` shader only ever runs on OpenGL, so apply the flip there directly.
