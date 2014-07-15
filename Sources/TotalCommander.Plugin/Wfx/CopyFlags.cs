using System;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Defines constants that are used by the 
    /// <see cref="ITotalCommanderWfxPlugin.FileGet"/> and <see cref="ITotalCommanderWfxPlugin.FilePut"/> methods.
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.FileGet"/>
    /// <seealso cref="ITotalCommanderWfxPlugin.FilePut"/>
    [Flags]
    public enum CopyFlags
    {
        /// <summary>
        /// Not set.
        /// </summary>
        None = 0,

        /// <summary>
        /// If set, overwrite any existing file without asking. If not set, simply fail copying.
        /// </summary>
        Overwrite = 1,

        /// <summary>
        /// Resume an aborted or failed transfer.
        /// </summary>
        Resume = 2,

        /// <summary>
        /// The plugin needs to delete the remote file after uploading.
        /// </summary>
        Move = 4,

        /// <summary>
        /// A hint from the calling program: The remote file exists and has the same case (upper/lowercase) as the local file.
        /// </summary>
        ExistsSameCase = 8,

        /// <summary>
        /// A hint from the calling program: The remote file exists and has different case (upper/lowercase) than the local file.
        /// </summary>
        ExistsDifferentCase = 16,

        /// <summary>
        /// A hint from the calling program: The remote file exists.
        /// </summary>
        Exists = ExistsDifferentCase | ExistsSameCase
    }
}
