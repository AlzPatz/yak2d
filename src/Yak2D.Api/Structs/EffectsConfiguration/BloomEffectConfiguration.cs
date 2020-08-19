namespace Yak2D
{
	/// <summary>
	/// The bloom effect downsamples the source, ignoring colour data below a brightness cut off
	/// The downsampled image is then blurred
	/// The final result is the source image plus a blend of the downsampled image back on top
	/// </summary>
	public struct BloomEffectConfiguration
	{
		/// <summary>
		/// The pixel intensity cut off amount (0 to 1)
		/// </summary>
		public float BrightnessThreshold { get; set; }

		/// <summary>
		/// The multiplication factor that the bloom sample is added back to the source image at
		/// </summary>
		public float AdditiveMixAmount { get; set; }

		/// <summary>
		/// Number of blur samples (MAX 8, will be capped)
		/// </summary>
		public int NumberOfBlurSamples { get; set; }

		/// <summary>
		/// Pixel sampling method (nearest neighbour, 2x2, 4x4)
		/// </summary>
		public ResizeSamplerType ReSamplerType { get; set; }
	}
}