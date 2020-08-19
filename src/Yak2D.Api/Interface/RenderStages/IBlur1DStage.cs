namespace Yak2D
{
	/// <summary>
	/// A RenderStage that applies a directional (1d) blur effect to a texture source and render to a target
	/// For non-directional blur, use the standard Blur effect instead
	/// Blur direction, fractional mix amount with source and number of blur component samples can be configured for the stage
	/// Holds the unsiged 64 bit integer framework id for the stage
	/// </summary>
	public interface  IBlur1DStage : IRenderStage
	{
		
	}
}