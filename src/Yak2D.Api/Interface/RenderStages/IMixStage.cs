namespace Yak2D
{
	/// <summary>
	/// A RenderStage that enables the mixing of up to four textures together
	/// The general mix factors per texture are defined in the configuration
	/// A further 'per-pixel' mixing scalar can be defined using mixTexture
	/// Holds the unsiged 64 bit integer framework id for the stage
	/// </summary>
	public interface IMixStage : IRenderStage
	{
		
	}
}