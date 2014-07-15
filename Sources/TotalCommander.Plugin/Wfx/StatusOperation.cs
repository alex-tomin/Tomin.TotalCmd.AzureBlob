
namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Defines constants that are returned by the <see cref="ITotalCommanderWfxPlugin.StatusInfo"/> method.
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.StatusInfo"/>
    /// <seealso cref="ITotalCommanderWfxPlugin.FindFirst"/>
    /// <seealso cref="ITotalCommanderWfxPlugin.FindNext"/>
    /// <seealso cref="ITotalCommanderWfxPlugin.FindClose"/>
    /// <seealso cref="ITotalCommanderWfxPlugin.FileGet"/>
    public enum StatusOperation
    {
        /// <summary>
        /// No operation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Retrieve a directory listing.
        /// </summary>
        List = 1,

        /// <summary>
        /// Get a single file from the plugin file system.
        /// </summary>
        GetSingle,

        /// <summary>
        /// Get multiple files, may include subdirs.
        /// </summary>
        GetMulti,

        /// <summary>
        /// Put a single file to the plugin file system.
        /// </summary>
        PutSingle,

        /// <summary>
        /// Put multiple files, may include subdirs.
        /// </summary>
        PutMulti,

        /// <summary>
        /// Rename/Move/Remote copy a single file.
        /// </summary>
        RenameMoveSingle,

        /// <summary>
        /// RenMov multiple files, may include subdirs.
        /// </summary>
        RenameMoveMulti,

        /// <summary>
        /// Delete multiple files, may include subdirs.
        /// </summary>
        Delete,

        /// <summary>
        /// Change attributes/times, may include subdirs.
        /// </summary>
        Attributes,

        /// <summary>
        /// Create a single directory.
        /// </summary>
        CreateDirectory,

        /// <summary>
        /// Start a single remote item, or a command line.
        /// </summary>
        Execute,

        /// <summary>
        /// Calculating size of subdir (user pressed SPACE).
        /// </summary>
        CalculateSize,

        /// <summary>
        /// Searching for file names only (using 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/>/<see cref="ITotalCommanderWfxPlugin.FindClose"/>).
        /// </summary>
        Search,

        /// <summary>
        /// Searching for file contents (using also <see cref="ITotalCommanderWfxPlugin.FileGet"/> calls).
        /// </summary>
        SearchText,

        /// <summary>
        /// Synchronize dirs searches subdirs for info.
        /// </summary>
        SyncSearch,

        /// <summary>
        /// Synchronize: Downloading files from plugin.
        /// </summary>
        SyncGet,

        /// <summary>
        /// Synchronize: Uploading files to plugin.
        /// </summary>
        SyncPut,

        /// <summary>
        /// Synchronize: Deleting files from plugin.
        /// </summary>
        SyncDelete,

        /// <summary>
        /// Get multiple files, may include subdirs in background thread.
        /// </summary>
        GetMultiThread,

        /// <summary>
        /// Put multiple files, may include subdirs in background thread.
        /// </summary>
        PutMultiThread
    }
}
