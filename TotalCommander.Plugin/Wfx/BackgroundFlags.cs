using System;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Defines constants that are used by the 
    /// <see cref="ITotalCommanderWfxPlugin.GetBackgroundFlags"/> method.
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.GetBackgroundFlags"/>
    [Flags]
    public enum BackgroundFlags
    {
        /// <summary>
        /// Background operations are not supported.
        /// </summary>
        NotSupported = 0,

        /// <summary>
        /// Download in background is supported.
        /// </summary>
        Download = 1,

        /// <summary>
        /// Upload in background is supported.
        /// </summary>
        Upload = 2,

        /// <summary>
        /// If this flag is set, the user must be asked BEFORE the transfer starts whether the file 
        /// should be sent/retrieved in background or foreground. 
        /// This is necessary for plugins where a separate connection is needed for background transfers,
        /// e.g. the SFTP (secure ftp) plugin.
        /// If the flag isn't set, there will be a "background" button in the actual progress window.
        /// </summary>
        AskUser = 4
    }
}
