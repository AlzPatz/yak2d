namespace Yak2D
{
	/// <summary>
	/// A RenderStage that applies stylised effects to a texture source and render to a target
	/// Effects are: Pixellate, Edge Detection, Cathode Ray Tube and Old Movie Reel, 
	/// A combination of all the effects can be applied together
	/// Holds the unsiged 64 bit integer framework id for the stage
	/// </summary>          
	public interface IStyleEffectsStage : IRenderStage
	{
		
	}
}