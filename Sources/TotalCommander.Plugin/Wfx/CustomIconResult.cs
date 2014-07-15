
namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Defines constants that are returned by the <see cref="ITotalCommanderWfxPlugin.GetCustomIcon"/>
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.GetCustomIcon"/>
    public enum CustomIconResult
    {
        /// <summary>
        /// No icon is returned. The calling app should show the default icon for this file type.
        /// </summary>
        UseDefault,

        /// <summary>
        /// An icon was returned. The icon must NOT be freed by the calling app.
        /// </summary>
        Extracted,

        /// <summary>
        /// An icon was returned. The icon MUST be destroyed by the calling app.
        /// </summary>
        ExtractedDestroy,

        /// <summary>
        /// This return value is only valid if <see cref="CustomIconFlags.Background"/> was NOT set. 
        /// It tells the calling app to show a default icon, and request the true icon in a background thread.
        /// </summary>
        Delayed
    }
}
