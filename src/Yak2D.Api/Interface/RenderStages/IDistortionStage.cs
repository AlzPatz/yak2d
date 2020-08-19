namespace Yak2D
{
    /// <summary>
    /// A RenderStage that applies a distortion effect to a texture source and render to a target
    /// The per-pixel distortion amount is generated from a distortion height map
    /// The distortion height map is drawn each frame, in a similar way that DrawStage draw requests are generated, queued and sorted
    /// The final distortion amount per-pixel is scaled by a configurable scalar quantity
	/// Holds the unsiged 64 bit integer framework id for the stage
    /// </summary>
    public interface IDistortionStage  :  IRenderStage
	{

	}
}