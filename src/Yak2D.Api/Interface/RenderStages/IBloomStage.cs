namespace Yak2D
{
	/// <summary>
	/// A RenderStage that applies a bloom effect to a texture source and render to a target
	/// Brightness threshold, additive mix amount with source and number of blur component samples can be configured for the stage
	/// The source texture is downsampled and threhold clipped onto a smaller surface before being blurred and re-mixed with the original
	/// Holds the unsiged 64 bit integer framework id for the stage
	/// </summary>
	public interface IBloomStage : IRenderStage
	{
		
	}
}