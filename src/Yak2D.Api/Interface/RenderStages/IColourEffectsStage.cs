namespace Yak2D
{
	/// <summary>
	/// A RenderStage that applies simple colour modification effects to a texture source and render to a target
	/// Effects are: single colour mix, grayscale, negative, colourise and opacity
	/// A combination of all the effects can be applied together
	/// Holds the unsiged 64 bit integer framework id for the stage
	/// </summary>
	public interface IColourEffectsStage : IRenderStage
	{
		
	}
}