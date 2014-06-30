
namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Defines constants that are used by the <see cref="ITotalCommanderWfxPlugin.StatusInfo"/> method.
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.StatusInfo"/>
    public enum StatusOrigin
	{
        /// <summary>
        /// Operation starts (allocate buffers if needed).
        /// </summary>
		Start,

        /// <summary>
        /// Operation has ended (free buffers, flush cache etc).
        /// </summary>
        End
	}
}
