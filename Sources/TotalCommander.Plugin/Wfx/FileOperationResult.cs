
namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Defines constants that are returned by the 
    /// <see cref="ITotalCommanderWfxPlugin.FileRenameMove"/>, 
    /// <see cref="ITotalCommanderWfxPlugin.FileGet"/> and
    /// <see cref="ITotalCommanderWfxPlugin.FilePut"/> methods.
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.FileRenameMove"/>
    /// <seealso cref="ITotalCommanderWfxPlugin.FileGet"/>
    /// <seealso cref="ITotalCommanderWfxPlugin.FilePut"/>
    public enum FileOperationResult
    {
        /// <summary>
        /// The file was copied OK
        /// </summary>
        OK,

        /// <summary>
        /// The local file already exists, and resume isn't supported.
        /// </summary>
        Exists,

        /// <summary>
        /// The remote file couldn't be found or opened.
        /// </summary>
        NotFound,

        /// <summary>
        /// There was an error reading from the remote file.
        /// </summary>
        ReadError,

        /// <summary>
        /// There was an error writing to the local file, e.g. disk full.
        /// </summary>
        WriteError,

        /// <summary>
        /// Copying was aborted by the user (through ProgressProc).
        /// </summary>
        UserAbort,

        /// <summary>
        /// The operation is not supported (e.g. resume).
        /// </summary>
        NotSupported,

        /// <summary>
        /// The local file already exists, and resume is supported.
        /// </summary>
        ExistsResumeAllowed,

        /// <summary>
        /// Default value equal <see cref="FileOperationResult.OK"/>
        /// </summary>
        Default = OK
    }
}
