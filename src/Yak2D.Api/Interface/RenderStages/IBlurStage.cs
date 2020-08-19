namespace Yak2D
{
	/// <summary>
	/// A RenderStage that applies a blur effect to a texture source and render to a target
	/// The blur is applied in both x and y texture coordinate directions, it is not a directional blur - use Blur1D effect instead in that scenario
	/// Fractional mix amount with source and number of blur component samples can be configured for the stage
	/// Holds the unsiged 64 bit integer framework id for the stage
	/// </summary>
	public interface  IBlurStage : IRenderStage
	{
		
	}
}