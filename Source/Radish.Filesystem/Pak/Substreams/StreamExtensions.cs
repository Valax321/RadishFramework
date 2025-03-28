namespace Radish.Filesystem.Pak.Substreams
{
	/// <summary>
	/// Provides <see cref="Stream"/> extension methods.
	/// </summary>
	internal static class StreamExtensions
	{
		/// <summary>
		/// Returns a substream instance sourcing from this stream at the specified offset with the specified length.
		/// </summary>
		/// <param name="stream">The sourcing stream.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="length">The length.</param>
		/// <returns>A substream.</returns>
		public static Substream Substream(this Stream stream, long offset, long length) => new Substream(stream, offset, length);
	}
}
