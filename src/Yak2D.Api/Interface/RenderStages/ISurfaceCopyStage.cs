namespace Yak2D
{
	/// <summary>
	/// A RenderStage that performs a GPU to CPU copy of Surface Pixel Data
	/// A Surface being either a Texture or RenderTarget
	/// Cannot copy from window main render target (TO CHECK)
	/// The stage requires the creation of an internal staging texture. This is sized to the source texture 
	/// The texture size can be pre-set to avoid texture creation at render, but it will be re-created if the 
	/// size does not match the eventual source texture copied from
	/// The Stage item itself holds the callback delegate that is called with the pixel data once rendering is complete
	/// Holds the unsiged 64 bit integer framework id for the stage
	/// </summary>
	public interface ISurfaceCopyStage : IRenderStage
	{

	}
}