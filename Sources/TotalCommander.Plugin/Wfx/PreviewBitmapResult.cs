
namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Defines constants that are returned by the <see cref="ITotalCommanderWfxPlugin.GetPreviewBitmap"/>
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.GetPreviewBitmap"/>
    public enum PreviewBitmapResult
    {
        /// <summary>
        /// There is no preview bitmap.
        /// </summary>
        None = 0,

        /// <summary>
        /// The image was extracted and is returned.
        /// </summary>
        Extracted,

        /// <summary>
        /// Tells the caller to extract the image by itself.
        /// </summary>
        ExtractYourSelf,

        /// <summary>
        /// Tells the caller to extract the image by itself, and then delete the temporary image file.
        /// </summary>
        ExtractYourSelfAndDelete,

        /// <summary>
        /// This value must be ADDED to one of the above values if the caller should cache the image.
        /// </summary>
        Cache = 256
    }
}
