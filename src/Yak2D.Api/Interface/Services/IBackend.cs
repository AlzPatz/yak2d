namespace Yak2D
{
	/// <summary>
	/// Platform API operations
	/// </summary>
	public interface IBackend
	{
		/// <summary>
		/// Returns the current Graphics API in use
		/// </summary>
		GraphicsApi GraphicsApi { get; }

		/// <summary>
		/// Returns whether a certain Graphis API is supported by the current platform
		/// </summary>
		/// <param name="api">The Graphis API to test</param>
		bool IsGraphicsApiSupported(GraphicsApi api);

		/// <summary>
		/// Sets new Graphis API (will not action if platform is not supported or requested API is the same as that currently in use)
		/// Note: upon any API change, all framework resources will be destroyed and a call to [re-]CreateResources in the application will be made
		/// </summary>
		/// <param name="api">The Graphis API requested</param>
		void SetGraphicsApi(GraphicsApi api);
	}
}