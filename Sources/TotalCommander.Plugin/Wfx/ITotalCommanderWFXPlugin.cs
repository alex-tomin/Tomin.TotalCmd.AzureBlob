using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Represents file system plugins for Total Commander.
    /// File system plugins will show up in Network Neighborhood, not in the new file system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <strong>The minimum functions needed for a read-only (browsing) plugin are:</strong>
    /// <list type="table">
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.Init"/></term>
    /// <description>Initialize the plugin.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FindFirst"/></term>
    /// <description>Retrieve the first file in a directory.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FindNext"/></term>
    /// <description>Get the next file in the directory.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FindClose"/></term>
    /// <description>Close the search handle.</description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>The following optional functions allow to manipulate individual files.</strong>
    /// <list type="table">
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FileGet"/></term>
    /// <description>Download a file from the plugin file system to a local disk.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FilePut"/></term>
    /// <description>Upload a file from the local disk to the plugin file system.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FileRenameMove"/></term>
    /// <description>Copy, rename or move a file within the plugin file system.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FileRemove"/></term>
    /// <description>Delete a file on the plugin file system.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FileExecute"/></term>
    /// <description>Execute a command or launch a file on the plugin file system, or show properties.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.DirectoryCreate"/></term>
    /// <description>Create a new directory in the plugin file system.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.DirectoryRemove"/></term>
    /// <description>Remove a directory on the plugin file system.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.SetFileAttributes"/></term>
    /// <description>Set the file attributes of a file or directory.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.SetFileTime"/></term>
    /// <description>Set the file times.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.GetCustomIcon"/></term>
    /// <description>Extract icon for file list.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.GetPreviewBitmap"/></term>
    /// <description>Return a bitmap for thumbnail view.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.Disconnect"/></term>
    /// <description>For file systems requiring a connection: User pressed disconnect button.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.StatusInfo"/></term>
    /// <description>Informs the plugin that an operation is just going to start or end (purely informational).</description>
    /// </item>
    /// <item>
    /// <term>Content*</term>
    /// <description>These functions are almost identical to the Content* functions of the content plugin interface.</description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>There are also 3 classes which the plugin can use:</strong>
    /// <list type="table">
    /// <item>
    /// <term><see cref="Progress"/></term>
    /// <description>Use this to indicate the progress in percent of a single copy operation.</description>
    /// </item>
    /// <item>
    /// <term><see cref="Log"/></term>
    /// <description>Use to add information to the log file, and to make the FTP toolbar appear.</description>
    /// </item>
    /// <item>
    /// <term><see cref="Request"/></term>
    /// <description>Request input from the user, e.g. a user name, password etc.</description>
    /// </item>
    /// </list>
    /// </para>
    /// <strong>How it works:</strong>
    /// <para>
    /// When a user installs the plugin in Total Commander, the plugin is loaded, class implemented interface 
    /// <see cref="ITotalCommanderWfxPlugin"/> is found and will be saved to wincmd.ini.
    /// Then the plugin will be unloaded.
    /// When the user enters Network Neighborhood, Totalcmd will enumerate all plugins listed in wincmd.ini without 
    /// loading the plugins! A plugin will only be loaded when the user tries to enter the plugin root directory.
    /// It's also possible that a user jumps directly to a plugin subdirectory by using the 'directory hotlist' or 
    /// 'directory history' functions in Totalcmd. When the plugin is loaded, 
    /// Totalcmd will call <see cref="ITotalCommanderWfxPlugin.Init"/>.
    /// </para>
    /// <para>
    /// The framework (Total Commander) will refresh the file list whenever the user enters any directory in the 
    /// plugin's file system. The same procedure will also be executed if the framework wants to work with subdirectories, 
    /// e.g. while copying/moving/deleting files in a subdir selected by the user. 
    /// This is done by recursively calling <see cref="ITotalCommanderWfxPlugin.FindFirst"/>, 
    /// <see cref="ITotalCommanderWfxPlugin.FindNext"/>, <see cref="ITotalCommanderWfxPlugin.FindClose"/> 
    /// for every directory encountered in the tree.
    /// This system will be called FNC (findfirst-next-close) in this text. 
    /// </para>
    /// <para>
    /// For the plugin root, Totalcmd calls <see cref="ITotalCommanderWfxPlugin.FindFirst"/> with the parameter 
    /// Path set to "\". The plugin should return all the items in the root, e.g. the drive letters of a 
    /// remote machine, the available file systems etc. When the returned item has the directory flag set, 
    /// Totalcmd will use the name to build a subdirectory path. Subdirectories are built by concatenating 
    /// returned directory names separated by Backslashes, e.g. \drive1\c:\some\subdir.
    /// </para>
    /// <para>
    /// While downloading or remote-copying whole directory trees, the framework executes a complete FNC loop of a 
    /// subdir and stores the files in an internal list. Then it checks the list for files and copies these files, and in a 
    /// second loop it checks the list for directories, and if it encounters them, it recursively copies the subdirs. 
    /// This allows to recursively copy a whole tree.
    /// </para>
    /// <para>
    /// For counting the files in subdirs and for deleting files, 
    /// multiple open file handles are needed. You should therefore initialise a temporary structure whenever 
    /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/> is called, return its handle (pointer) to the framework, 
    /// and delete it in <see cref="ITotalCommanderWfxPlugin.FindClose"/>, using that same handle that is now returned to you. 
    /// It's important to know that there may be multiple open find handles a the same time, although great care is taken to avoid this.
    /// </para>
    /// <para>
    /// Some framework function may call other functions when the need arises - for instance, <see cref="ITotalCommanderWfxPlugin.DirectoryRemove"/>
    /// is called during moving of files in order to delete the directories that are no longer needed. 
    /// </para>
    /// <para>
    /// Here are some cases when you CAN'T rely on the FNC to get called (because it has already been called before):<br />
    /// - when copying some files in the currently active directory, and there are no directories selected for copying<br />
    /// - when viewing a file with F3
    /// </para>
    /// <para>
    /// By <see cref="ITotalCommanderWfxPlugin.StatusInfo"/> method, the plugin will be informed every time an operation starts and ends. 
    /// No plugin functions except for <see cref="ITotalCommanderWfxPlugin.Init"/> and <see cref="ITotalCommanderWfxPlugin.Disconnect"/> 
    /// will be called without an enclosing pair of <see cref="ITotalCommanderWfxPlugin.StatusInfo"/> calls.
    /// </para>
    /// </remarks>
    public interface ITotalCommanderWfxPlugin
    {
        /// <summary>
        /// The default root name which should appear in the Network Neighborhood.
        /// </summary>
        /// <remarks>
        /// This root name is NOT part of the path passed to the plugin when Totalcmd accesses the 
        /// plugin file system! The root will always be "\", and all subpaths will be built from 
        /// the directory names returned by the plugin.
        /// </remarks>
        string PluginName
        {
            get;
        }

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.Init"/> is called when loading the plugin.
        /// The passed values should be stored in the plugin for later use.
        /// </summary>
        /// <param name="pluginNumber">Internal number this plugin was given in Total Commander.</param>
        /// <param name="progress"><see cref="Progress"/> class, which contains progress functions.</param>
        /// <param name="log"><see cref="Log"/> class, which contains logging functions.</param>
        /// <param name="request"><see cref="Request"/> class, which contains request text functions.</param>
        /// <remarks>
        /// <see cref="ITotalCommanderWfxPlugin.Init"/> is NOT called when the user initially installs the plugin.
        /// The plugin DLL is loaded when the user enters the plugin root in Network Neighborhood.
        /// </remarks>
        /// <seealso cref="ITotalCommanderWfxPlugin.SetDefaultParams"/>
        void Init(int pluginNumber, Progress progress, Log log, Request request);


        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/> is called to retrieve the first file in a directory of the plugin's file system.
        /// </summary>
        /// <param name="path">
        /// Full path to the directory for which the directory listing has to be retrieved.
        /// Important: no wildcards are passed to the plugin!
        /// All separators will be backslashes, so you will need to convert them to forward slashes if your file system uses them!
        /// As root, a single backslash is passed to the plugin.
        /// All subdirs are built from the directory names the plugin returns through 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/> and <see cref="ITotalCommanderWfxPlugin.FindNext"/>,
        /// separated by single backslashes, e.g. \Some server\c:\subdir
        /// </param>
        /// <param name="enumerator">
        /// The find enumerator. This enumerator will be passed to <see cref="ITotalCommanderWfxPlugin.FindNext"/> by the calling program.
        /// </param>
        /// <returns>
        /// <list type="number">
        /// <item><term>Return <see cref="FindData"/> class, which contains the file or directory details.
        /// Use the <see cref="FindData.Attributes"/> property set to <see cref="System.IO.FileAttributes.Directory"/> to distinguish files from directories.
        /// On Unix systems, you can | (or) the <see cref="FindData.Attributes"/> property with 0x80000000 and
        /// set the <see cref="FindData.Reserved0"/> property to the Unix file mode (permissions).</term></item>
        /// <item><term>Return <see cref="FindData.NoMoreFiles"/> if the directory exists, but it's empty (Totalcmd can open it, e.g. to copy files to it).</term></item>
        /// <item><term>Return <see cref="FindData.NotOpen"/> if the directory does not exist, and Total Commander will not try to open it.</term></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/> may be called directly with a subdirectory of the plugin!
        /// You cannot rely on it being called with the root \ after it is loaded.
        /// Reason: Users may have saved a subdirectory to the plugin in the Ctrl+D directory hotlist in a previous session with the plugin.
        /// </remarks>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindNext"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindClose"/>
        FindData FindFirst(string path, out IEnumerator enumerator);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FindNext"/> is called to retrieve the next file in a directory of the plugin's file system.
        /// </summary>
        /// <param name="enumerator">The find enumerator returned by <see cref="ITotalCommanderWfxPlugin.FindFirst"/>.</param>
        /// <returns>
        /// <list type="number">
        /// <item><term>Return <see cref="FindData"/> class, which contains the file or directory details.
        /// Use the <see cref="FindData.Attributes"/> property set to <see cref="System.IO.FileAttributes.Directory"/> to distinguish files from directories.
        /// On Unix systems, you can | (or) the <see cref="FindData.Attributes"/> property with 0x80000000 and
        /// set the <see cref="FindData.Reserved0"/> property to the Unix file mode (permissions).</term></item>
        /// <item><term>Return <see cref="FindData.NoMoreFiles"/> if there are no more files.</term></item>
        /// </list>
        /// </returns>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindFirst"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindClose"/>
        FindData FindNext(IEnumerator enumerator);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FindClose"/> is called to end a 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/> loop, 
        /// either after retrieving all files, or when the user aborts it.
        /// </summary>
        /// <param name="enumerator">The find enumerator returned by <see cref="ITotalCommanderWfxPlugin.FindFirst"/>.</param>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindFirst"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindNext"/>
        void FindClose(IEnumerator enumerator);


        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FileExecute"/> is called to execute a file on the plugin's file system,
        /// or show its property sheet. It is also called to show a plugin configuration dialog when the 
        /// user right clicks on the plugin root and chooses 'properties'.
        /// The plugin is then called with <paramref name="remoteName"/>="\" and <paramref name="verb"/>="properties" 
        /// (requires TC>=5.51).
        /// </summary>
        /// <param name="window">Parent window which can be used for showing a property sheet.</param>
        /// <param name="remoteName">Name of the file to be executed, with full path.</param>
        /// <param name="verb">
        /// This can be either "open", "properties", "chmod" or "quote" (case-insensitive).
        /// <para>
        /// <strong>open:</strong> This is called when the user presses ENTER on a file. There are three ways to handle it:<br />
        /// a) For internal commands like "Add new connection", execute it in the plugin and return 
        /// <see cref="ExecuteResult.OK"/> or <see cref="ExecuteResult.Error"/><br />
        /// b) Let Total Commander download the file and execute it locally: return <see cref="ExecuteResult.YourSelf"/><br />
        /// c) If the file is a (symbolic) link, set <paramref name="remoteName"/> to the location to which the link points 
        /// (including the full plugin path), and return <see cref="ExecuteResult.SymLink"/>. 
        /// Total Commander will then switch to that directory. You can also switch to a directory on the local harddisk! 
        /// To do this, return a path starting either with a drive letter, or an UNC location (\\server\share). 
        /// The maximum allowed length of such a path is MAX_PATH-1 = 259 characters!
        /// </para>
        /// <para>
        /// <strong>properties:</strong> Show a property sheet for the file (optional). Currently not handled by internal Totalcmd functions 
        /// if <see cref="ExecuteResult.YourSelf"/> is returned, so the plugin needs to do it internally.
        /// </para>
        /// <para>
        /// <strong>chmod xxx:</strong> The xxx stands for the new Unix mode (attributes) to be applied to the file 
        /// <paramref name="remoteName"/>. This verb is only used when returning Unix attributes through 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/>
        /// </para>
        /// <para>
        /// <strong>quote commandline:</strong> Execute the command line entered by the user in the directory <paramref name="remoteName"/>.
        /// This is called when the user enters a command in Totalcmd's command line, and presses ENTER.
        /// This is optional, and allows to send plugin-specific commands. It's up to the plugin writer what 
        /// to support here. If the user entered e.g. a cd directory command, you can return the new path in 
        /// <paramref name="remoteName"/> (max 259 characters), and give <see cref="ExecuteResult.SymLink"/> as return value.
        /// Return <see cref="ExecuteResult.OK"/> to cause a refresh (re-read) of the active panel.
        /// </para>
        /// </param>
        /// <returns>
        /// <list type="table">
        /// <item>
        /// <term><see cref="ExecuteResult.YourSelf"/></term>
        /// <description>Total Commander should download the file and execute it locally.</description>
        /// </item>
        /// <item>
        /// <term><see cref="ExecuteResult.OK"/></term>
        /// <description>The command was executed successfully in the plugin 
        /// (or if the command isn't applicable and no further action is needed).</description>
        /// </item>
        /// <item>
        /// <term><see cref="ExecuteResult.Error"/></term>
        /// <description>Execution failed.</description>
        /// </item>
        /// <item>
        /// <term><see cref="ExecuteResult.SymLink"/></term>
        /// <description>This was a (symbolic) link or .lnk file pointing to a different directory.</description>
        /// </item>
        /// </list>
        /// </returns>
        ExecuteResult FileExecute(TotalCommanderWindow window, ref string remoteName, string verb);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FileRenameMove"/> is called to transfer 
        /// (copy or move) a file within the plugin's file system.
        /// </summary>
        /// <param name="oldName">Name of the remote source file, with full path. The name always 
        /// starts with a backslash, then the names returned by 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/> 
        /// separated by backslashes.</param>
        /// <param name="newName">Name of the remote destination file, with full path. The name always starts 
        /// with a backslash, then the names returned by 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/> 
        /// separated by backslashes.</param>
        /// <param name="move">If true, the file needs to be moved to the new location and name. 
        /// Many file systems allow to rename/move a file without actually moving any of its data,
        /// only the pointer to it.</param>
        /// <param name="overwrite">Tells the function whether it should overwrite the target file 
        /// or not. See notes below on how this parameter is used.</param>
        /// <param name="ri">A structure of type <see cref="RemoteInfo"/> which contains 
        /// the parameters of the file being renamed/moved (not of the target file!).</param>
        /// <returns>Return one of the following values:
        /// <list type="table">
        /// <item>
        /// <term><see cref="FileOperationResult.OK"/></term>
        /// <description>The file was copied/moved OK.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.Exists"/></term>
        /// <description>The target file already exists.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotFound"/></term>
        /// <description>The source file couldn't be found or opened.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ReadError"/></term>
        /// <description>There was an error reading from the source file.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.WriteError"/></term>
        /// <description>There was an error writing to the target file, e.g. disk full.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.UserAbort"/></term>
        /// <description>Copying was aborted by the user (through Progress).</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotSupported"/></term>
        /// <description>The operation is not supported (e.g. resume).</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ExistsResumeAllowed"/></term>
        /// <description>Not used here.</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Total Commander usually calls this function twice:<br />
        /// - once with <paramref name="overwrite"/> == <strong>false</strong>. 
        /// If the remote file exists, return <see cref="FileOperationResult.Exists"/>. 
        /// If it doesn't exist, try to copy the file, and return an appropriate error code.<br />
        /// - a second time with <paramref name="overwrite"/> == <strong>true</strong>, 
        /// if the user chose to overwrite the file.<br />
        /// While copying the file, but at least at the beginning and the end, 
        /// call <see cref="Progress.SetProgress"/> to show the copy progress and allow 
        /// the user to abort the operation.
        /// </remarks>
        FileOperationResult FileRenameMove(string oldName, string newName, bool move, bool overwrite, RemoteInfo ri);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FileGet"/> is called to transfer a file from 
        /// the plugin's file system to the normal file system (drive letters or UNC).
        /// </summary>
        /// <param name="remoteName">Name of the file to be retrieved, with full path. The name always 
        /// starts with a backslash, then the names returned by 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/> 
        /// separated by backslashes.</param>
        /// <param name="localName">Local file name with full path, either with a drive 
        /// letter or UNC path (\\Server\Share\filename). The plugin may change the 
        /// NAME/EXTENSION of the file (e.g. when file conversion is done), but not the path!</param>
        /// <param name="copyFlags">Can be a combination of the following three flags:<br />
        /// <see cref="CopyFlags.Overwrite"/>: If set, overwrite any existing file without asking.
        /// If not set, simply fail copying.<br />
        /// <see cref="CopyFlags.Resume"/>: Resume an aborted or failed transfer.<br />
        /// <see cref="CopyFlags.Move"/>: The plugin needs to delete the remote file after uploading.<br />
        /// See below for important notes!</param>
        /// <param name="ri">This parameter contains information about the remote file which was previously 
        /// retrieved via <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/>:
        /// The size, date/time, and attributes of the remote file. May be useful to copy 
        /// the attributes with the file, and for displaying a progress dialog.</param>
        /// <returns>Return one of the following values:
        /// <list type="table">
        /// <item>
        /// <term><see cref="FileOperationResult.OK"/></term>
        /// <description>The file was copied/moved OK.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.Exists"/></term>
        /// <description>The target file already exists.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotFound"/></term>
        /// <description>The source file couldn't be found or opened.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ReadError"/></term>
        /// <description>There was an error reading from the source file.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.WriteError"/></term>
        /// <description>There was an error writing to the target file, e.g. disk full.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.UserAbort"/></term>
        /// <description>Copying was aborted by the user (through Progress).</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotSupported"/></term>
        /// <description>The operation is not supported (e.g. resume).</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ExistsResumeAllowed"/></term>
        /// <description>The remote file already exists, and resume is supported.</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <para>
        /// Total Commander usually calls this function twice:<br />
        /// - once with <paramref name="copyFlags"/> == <see cref="CopyFlags.None"/> or 
        /// <paramref name="copyFlags"/> == <see cref="CopyFlags.Move"/>. 
        /// If the local file exists and resume is supported, 
        /// return <see cref="FileOperationResult.ExistsResumeAllowed"/>.
        /// If resume isn't allowed, return <see cref="FileOperationResult.Exists"/>.<br />
        /// - a second time with <see cref="CopyFlags.Resume"/> or <see cref="CopyFlags.Overwrite"/>, 
        /// depending on the user's choice. The resume option is only offered to the user 
        /// if <see cref="FileOperationResult.ExistsResumeAllowed"/> was returned by the first call.<br />
        /// - <see cref="CopyFlags.ExistsSameCase"/> and <see cref="CopyFlags.ExistsDifferentCase"/>
        /// are NEVER passed to this function, because the plugin can easily 
        /// determine whether a local file exists or not.<br />
        /// - <see cref="CopyFlags.Move"/> is set, the plugin needs to delete the remote file after a successful download.
        /// </para>
        /// <para>
        /// While copying the file, but at least at the beginning and the end, call 
        /// ProgressProc to show the copy progress and allow the user 
        /// to abort the operation.
        /// </para>
        ///</remarks>
        FileOperationResult FileGet(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FilePut"/> is called to transfer a file from 
        /// the normal file system (drive letters or UNC) to the plugin's file system.
        /// </summary>
        /// <param name="localName">
        /// Local file name with full path, either with a drive letter or 
        /// UNC path (\\Server\Share\filename). This file needs to be uploaded to the plugin's file system.
        /// </param>
        /// <param name="remoteName">
        /// Name of the remote file, with full path. The name always 
        /// starts with a backslash, then the names returned by 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/> 
        /// separated by backslashes. The plugin may change the NAME/EXTENSION of the file 
        /// (e.g. when file conversion is done), but not the path!
        /// </param>
        /// <param name="copyFlags">
        /// Can be a combination of the following three flags:<br />
        /// <see cref="CopyFlags.Overwrite"/>: If set, overwrite any existing file without asking.
        /// If not set, simply fail copying.<br />
        /// <see cref="CopyFlags.Resume"/>: Resume an aborted or failed transfer.<br />
        /// <see cref="CopyFlags.Move"/>: The plugin needs to delete the remote file after uploading.<br />
        /// <see cref="CopyFlags.ExistsSameCase"/>: A hint from the calling program: The remote file exists and 
        /// has the same case (upper/lowercase) as the local file.<br />
        /// <see cref="CopyFlags.ExistsDifferentCase"/>:  A hint from the calling program: The remote file exists and 
        /// has different case (upper/lowercase) than the local file.<br />
        /// See below for important notes!
        /// </param>
        /// <returns>Return one of the following values:
        /// <list type="table">
        /// <item>
        /// <term><see cref="FileOperationResult.OK"/></term>
        /// <description>The file was copied/moved OK.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.Exists"/></term>
        /// <description>The target file already exists.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotFound"/></term>
        /// <description>The source file couldn't be found or opened.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ReadError"/></term>
        /// <description>There was an error reading from the source file.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.WriteError"/></term>
        /// <description>There was an error writing to the target file, e.g. disk full.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.UserAbort"/></term>
        /// <description>Copying was aborted by the user (through Progress).</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotSupported"/></term>
        /// <description>The operation is not supported (e.g. resume).</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ExistsResumeAllowed"/></term>
        /// <description>The remote file already exists, and resume is supported.</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <para>
        /// Total Commander usually calls this function twice, with the following parameters in <paramref name="copyFlags"/>:<br />
        /// - once with neither <see cref="CopyFlags.Resume"/> nor <see cref="CopyFlags.Overwrite"/> set.
        /// If the remote file exists and resume is supported, return <see cref="FileOperationResult.ExistsResumeAllowed"/>.
        /// If resume isn't allowed, return <see cref="FileOperationResult.Exists"/><br />
        /// - a second time with <see cref="CopyFlags.Resume"/> or <see cref="CopyFlags.Overwrite"/>, 
        /// depending on the user's choice. The resume option is only offered to the user 
        /// if <see cref="FileOperationResult.ExistsResumeAllowed"/> was returned by the first call.<br />
        /// - The flags <see cref="CopyFlags.ExistsSameCase"/> or <see cref="CopyFlags.ExistsDifferentCase"/> 
        /// are added to <paramref name="copyFlags"/> when the remote file exists and needs to be overwritten. 
        /// This is a hint to the plugin to allow optimizations: Depending on the plugin type, 
        /// it may be very slow to check the server for every single file when uploading.<br />
        /// - If the flag <see cref="CopyFlags.Move"/> is set, the plugin needs to delete the local 
        /// file after a successful upload.
        /// </para>
        /// <para>
        /// While copying the file, but at least at the beginning and the end, call 
        /// <see cref="Progress.SetProgress"/> to show the copy progress and allow the user to abort the operation.
        /// </para>
        ///</remarks>
        FileOperationResult FilePut(string localName, ref string remoteName, CopyFlags copyFlags);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FileRemove"/> is called to delete a file from the plugin's file system.
        /// </summary>
        /// <param name="remoteName">
        /// Name of the file to be deleted, with full path. The name always starts with a backslash, 
        /// then the names returned by 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/>
        /// separated by backslashes.
        /// </param>
        /// <returns>Return <strong>true</strong> if the file could be deleted, <strong>false</strong> if not.</returns>
        bool FileRemove(string remoteName);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.DirectoryCreate"/> is called to create a directory on the plugin's file system.
        /// </summary>
        /// <param name="path">
        /// Name of the directory to be created, with full path. 
        /// The name always starts with a backslash, then the names returned by 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/> separated by backslashes.
        /// </param>
        /// <returns>Return <strong>true</strong> if the directory could be created, <strong>false</strong> if not.</returns>
        /// <seealso cref="ITotalCommanderWfxPlugin.DirectoryRemove"/>
        bool DirectoryCreate(string path);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.DirectoryRemove"/> is called to remove a directory from the plugin's file system.
        /// </summary>
        /// <param name="remoteName">
        /// Name of the directory to be removed, with full path.
        /// The name always starts with a backslash, then the names returned by 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/> separated by backslashes.
        /// </param>
        /// <returns>Return <strong>true</strong> if the directory could be removed, <strong>false</strong> if not.</returns>
        /// <seealso cref="ITotalCommanderWfxPlugin.DirectoryCreate"/>
        bool DirectoryRemove(string remoteName);


        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.SetDefaultParams"/> is called immediately after <see cref="ITotalCommanderWfxPlugin.Init"/>.
        /// This function is new in version 1.3. It requires Total Commander >=5.51, but is ignored by older versions.
        /// </summary>
        /// <param name="defaultParam">This class of type <see cref="DefaultParam"/> currently contains the version number of the plugin interface,
        /// and the suggested location for the settings file (ini file).
        /// It is recommended to store any plugin-specific information either directly in that file, or in that directory under a different name.
        /// Make sure to use a unique header when storing data in this file, because it is shared by other file system plugins!
        /// If your plugin needs more than 1kbyte of data, you should use your own ini file because ini files are limited to 64k.
        /// </param>
        /// <remarks>
        /// This function is only called in Total Commander 5.51 and later. The plugin version will be >= 1.3.
        /// </remarks>
        /// <seealso cref="ITotalCommanderWfxPlugin.Init"/>
        void SetDefaultParams(DefaultParam defaultParam);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.SetFileAttributes"/> is called to set the (Windows-Style) file attributes of a file/dir.
        /// <see cref="ITotalCommanderWfxPlugin.FileExecute"/> is called for Unix-style attributes.
        /// </summary>
        /// <param name="remoteName">Name of the file/directory whose attributes have to be set.</param>
        /// <param name="attributes">New file attributes. These are a commbination of the following standard file attributes: 
        /// <see cref="FileAttributes.ReadOnly"/>, <see cref="FileAttributes.Hidden"/>, <see cref="FileAttributes.System"/>, <see cref="FileAttributes.Archive"/>.</param>
        /// <returns>Return <strong>true</strong> if successful, <strong>false</strong> if the function failed.</returns>
        /// <seealso cref="ITotalCommanderWfxPlugin.FileExecute"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.SetFileTime"/>
        bool SetFileAttributes(string remoteName, FileAttributes attributes);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.SetFileTime"/> is called to set the (Windows-Style) file times of a file/dir.
        /// </summary>
        /// <param name="remoteName">Name of the file/directory whose attributes have to be set.</param>
        /// <param name="creationTime">Creation time of the file. May be NULL to leave it unchanged.</param>
        /// <param name="lastAccessTime">Last access time of the file. May be NULL to leave it unchanged.</param>
        /// <param name="lastWriteTime">Last write time of the file. May be NULL to leave it unchanged. If your file system only supports one time, use this parameter!</param>
        /// <returns>Return <strong>true</strong> if successful, <strong>false</strong> if the function failed.</returns>
        /// <seealso cref="ITotalCommanderWfxPlugin.SetFileAttributes"/>
        bool SetFileTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.SetPasswordStore"/> is called when loading the plugin.
        /// The passed value should be stored in the plugin for later use. 
        /// </summary>
        /// <param name="password">
        /// You can use this class to store passwords in Total Commander's secure password store.
        /// The user will be asked for the master password automatically.
        /// </param>
        void SetPasswordStore(Password password);


        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.GetBackgroundFlags"/> is called by Total Commander 7.51 or 
        /// newer to determine whether the plugin supports background operations (uploads and downloads), 
        /// and if yes, how they are supported.
        /// </summary>
        /// <returns>
        /// A combination of the following flags:<br />
        /// <list type="table">
        /// <item>
        /// <term><see cref="BackgroundFlags.NotSupported"/></term>
        /// <description>Background operations are not supported.</description>
        /// </item>
        /// <item>
        /// <term><see cref="BackgroundFlags.Download"/></term>
        /// <description>Download in background is supported.</description>
        /// </item>
        /// <item>
        /// <term><see cref="BackgroundFlags.Upload"/></term>
        /// <description>Upload in background is supported.</description>
        /// </item>
        /// <item>
        /// <term><see cref="BackgroundFlags.AskUser"/></term>
        /// <description>
        /// If this flag is set, the user must be asked BEFORE the transfer starts whether the file 
        /// should be sent/retrieved in background or foreground. 
        /// This is necessary for plugins where a separate connection is needed for background transfers,
        /// e.g. the SFTP (secure ftp) plugin.
        /// If the flag isn't set, there will be a "background" button in the actual progress window.
        /// </description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <para>
        /// If the flag <see cref="BackgroundFlags.AskUser"/> is set and the user checks the option to 
        /// transfer the file in background, Total Commander starts a background thread and calls 
        /// <see cref="ITotalCommanderWfxPlugin.StatusInfo"/> with parameters 
        /// <see cref="StatusOrigin.Start"/> and <see cref="StatusOperation.GetMultiThread"/> instead of 
        /// <see cref="StatusOperation.GetMulti"/> for downloads, and 
        /// <see cref="StatusOperation.PutMultiThread"/> instead of <see cref="StatusOperation.PutMulti"/>
        /// for uploads. A corresponding <see cref="StatusOrigin.End"/> is sent when the 
        /// transfer is done. These notifications can be used to build up the background connection 
        /// and to close it when done. You need to use 
        /// <see cref="System.Threading.Thread.ManagedThreadId"/> to recognize a 
        /// background operation. <see cref="System.Threading.Thread.ManagedThreadId"/> 
        /// will return the same value for the entire operation in 
        /// <see cref="ITotalCommanderWfxPlugin.StatusInfo"/>, 
        /// <see cref="ITotalCommanderWfxPlugin.FileGet"/> and 
        /// <see cref="ITotalCommanderWfxPlugin.FilePut"/>.
        /// </para>
        /// <para>
        /// If the flag <see cref="BackgroundFlags.AskUser"/> is NOT set, all uploads and downloads 
        /// with F5 or F6 will be started in a background thread. Initially the normal foreground 
        /// transfer window will be shown, which will be changed to a background transfer window 
        /// when the user clicks on "Background". For the plugin, the entire operation will take 
        /// place in the same background thread, though. This method is recommended only for 
        /// plugins where no extra connections are needed for multiple parallel transfers, or where 
        /// the additional connections are built up very quickly. Example of a plugin using this 
        /// method is the WinCE plugin for Windows Mobile devices.
        /// </para>
        /// </remarks>
        /// <seealso cref="ITotalCommanderWfxPlugin.StatusInfo"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.FilePut"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.FileGet"/>
        BackgroundFlags GetBackgroundFlags();

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.GetCustomIcon"/> is called when a file/directory is displayed in the file list. 
        /// It can be used to specify a custom icon for that file/directory. 
        /// This function is new in version 1.1. It requires Total Commander >=5.51, but is ignored by older versions.
        /// </summary>
        /// <param name="remoteName">
        /// This is the full path to the file or directory whose icon is to be retrieved. 
        /// When extracting an icon, you can return an icon name here - this ensures that the icon is only cached once in the calling program. 
        /// The returned icon name must not be longer than MAX_PATH characters (including terminating 0!). 
        /// The icon handle must still be returned in <paramref name="icon"/>!</param>
        /// <param name="extractIconFlag">
        /// Flags for the extract operation. A combination of the following:<br />
        /// <see cref="CustomIconFlags.Small"/>: Requests the small 16x16 icon<br />
        /// <see cref="CustomIconFlags.Background"/>: The function is called from the background thread (see note below).
        /// </param>
        /// <param name="icon">
        /// Here you need to return the icon.
        /// </param>
        /// <returns>
        /// The function has to return one of the following values:<br />
        /// <list type="table">
        /// <item>
        /// <term><see cref="CustomIconResult.UseDefault"/></term>
        /// <description>No icon is returned. The calling app should show the default icon for this file type.</description>
        /// </item>
        /// <item>
        /// <term><see cref="CustomIconResult.Extracted"/></term>
        /// <description>An icon was returned in <paramref name="icon"/>. The icon must NOT be freed by the calling app, 
        /// e.g. because it was loaded with LoadIcon, or the DLL handles destruction of the icon.</description>
        /// </item>
        /// <item>
        /// <term><see cref="CustomIconResult.ExtractedDestroy"/></term>
        /// <description>An icon was returned in <paramref name="icon"/>. The icon MUST be destroyed by the calling app, 
        /// e.g. because it was created with CreateIcon(), or extracted with ExtractIconEx().</description>
        /// </item>
        /// <item>
        /// <term><see cref="CustomIconResult.Delayed"/></term>
        /// <description>This return value is only valid if <see cref="CustomIconFlags.Background"/> was NOT set. 
        /// It tells the calling app to show a default icon, and request the true icon in a background thread. 
        /// See note below.</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// If you return <see cref="CustomIconResult.Delayed"/>, <see cref="ITotalCommanderWfxPlugin.GetCustomIcon"/>
        /// will be called again from a background thread at a later time. 
        /// A critical section is used by the calling app to ensure that <see cref="ITotalCommanderWfxPlugin.GetCustomIcon"/>
        /// is never entered twice at the same time. This return value should be used for icons which take a while to extract, 
        /// e.g. EXE icons. In the fsplugin sample plugin, the drive icons are returned immediately 
        /// (because they are stored in the plugin itself), but the EXE icons are loaded with a delay. 
        /// If the user turns off background loading of icons, the function will be called 
        /// in the foreground with the <see cref="CustomIconFlags.Background"/> flag.
        /// </remarks>
        CustomIconResult GetCustomIcon(ref string remoteName, CustomIconFlags extractIconFlag, out Icon icon);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.GetPreviewBitmap"/> is called when a file/directory is displayed in thumbnail view. 
        /// It can be used to return a custom bitmap for that file/directory. 
        /// This function is new in version 1.4. It requires Total Commander >=7.0, but is ignored by older versions.
        /// </summary>
        /// <param name="remoteName">
        /// This is the full path to the file or directory whose bitmap is to be retrieved. 
        /// When extracting a bitmap, you can return a bitmap name here - this ensures that the icon is only cached once in the calling program. 
        /// The returned bitmap name must not be longer than MAX_PATH characters (including terminating 0!). 
        /// The bitmap handle must still be returned in <paramref name="bitmap"/>!
        /// </param>
        /// <param name="size">
        /// The maximum dimensions of the preview bitmap. If your image is smaller, or has a different side ratio, 
        /// then you need to return an image which is smaller than these dimensions! See notes below!
        /// </param>
        /// <param name="bitmap">
        /// Here you need to return the bitmap.
        /// </param>
        /// <returns>
        /// The function has to return one of the following values:<br />
        /// <list type="table">
        /// <item>
        /// <term><see cref="PreviewBitmapResult.None"/></term>
        /// <description>There is no preview bitmap.</description>
        /// </item>
        /// <item>
        /// <term><see cref="PreviewBitmapResult.Extracted"/></term>
        /// <description>The image was extracted and is returned in <paramref name="bitmap"/>.</description>
        /// </item>
        /// <item>
        /// <term><see cref="PreviewBitmapResult.ExtractYourSelf"/></term>
        /// <description>Tells the caller to extract the image by itself. 
        /// The full local path to the file needs to be returned in <paramref name="remoteName"/>. 
        /// The returned bitmap name must not be longer than MAX_PATH.</description>
        /// </item>
        /// <item>
        /// <term><see cref="PreviewBitmapResult.ExtractYourSelfAndDelete"/></term>
        /// <description>Tells the caller to extract the image by itself, and then delete the temporary image file. 
        /// The full local path to the temporary image file needs to be returned in <paramref name="remoteName"/>. 
        /// The returned bitmap name must not be longer than MAX_PATH. 
        /// In this case, the plugin downloads the file to TEMP and then asks TC to extract the image.</description>
        /// </item>
        /// <item>
        /// <term><see cref="PreviewBitmapResult.Cache"/></term>
        /// <description>This value must be ADDED to one of the above values if the caller should cache the image. 
        /// Do NOT add this image if you will cache the image yourself!</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <list type="number">
        /// <item>
        /// <term>This function is only called in Total Commander 7.0 and later. The reported plugin version will be >= 1.4.</term>
        /// </item>
        /// <item>
        /// <term>The bitmap handle goes into possession of Total Commander, which will delete it after using it. 
        /// The plugin must not delete the bitmap handle!</term>
        /// </item>
        /// <item>
        /// <term>This function is only called in Total Commander 7.0 and later. The reported plugin version will be >= 1.4.</term>
        /// </item>
        /// <item>
        /// <term>Make sure you scale your image correctly to the desired maximum width+height! 
        /// Do not fill the rest of the bitmap - instead, create a bitmap which is SMALLER than requested! 
        /// This way, Total Commander can center your image and fill the rest with the default background color.</term>
        /// </item>
        /// </list>
        /// <example>
        /// The following sample code will stretch a bitmap with dimensions bigwidth*bigheight down to max. 
        /// width*height keeping the correct aspect ratio (proportions):
        /// <code>
        /// int __stdcall FsGetPreviewBitmap(char* RemoteName,int width,int height, HBITMAP* ReturnedBitmap)
        /// {
        ///     int w,h,bigx,bigy;
        ///     int stretchx,stretchy;
        ///     OSVERSIONINFO vx;
        ///     BOOL is_nt;
        ///     BITMAP bmpobj;
        ///     HBITMAP bmp_image,bmp_thumbnail,oldbmp_image,oldbmp_thumbnail;
        ///     HDC maindc,dc_thumbnail,dc_image;
        ///     POINT pt;
        ///     
        ///     // check for operating system: Windows 9x does NOT support the HALFTONE stretchblt mode!
        ///     vx.dwOSVersionInfoSize=sizeof(vx);
        ///     GetVersionEx(&amp;vx);
        ///     is_nt=vx.dwPlatformId==VER_PLATFORM_WIN32_NT;
        ///     
        ///     // here you load your image
        ///     bmp_image=SomeHowLoadImageFromFile(RemoteName);
        ///     
        ///     if (bmp_image &amp;&amp; GetObject(bmp_image,sizeof(bmpobj),&amp;bmpobj)) {
        ///         bigx=bmpobj.bmWidth;
        ///         bigy=bmpobj.bmHeight;
        ///         // do we need to stretch?
        ///         if ((bigx&gt;=width || bigy&gt;=height) &amp;&amp; (bigx&gt;0 &amp;&amp; bigy&gt;0)) {
        ///             stretchy=MulDiv(width,bigy,bigx);
        ///             if (stretchy&lt;=height) {
        ///                 w=width;
        ///                 h=stretchy;
        ///                 if (h&lt;1) h=1;
        ///             } else {
        ///                 stretchx=MulDiv(height,bigx,bigy);
        ///                 w=stretchx;
        ///                 if (w&lt;1) w=1;
        ///                 h=height;
        ///             }
        ///             
        ///             maindc=GetDC(GetDesktopWindow());
        ///             dc_thumbnail=CreateCompatibleDC(maindc);
        ///             dc_image=CreateCompatibleDC(maindc);
        ///             bmp_thumbnail=CreateCompatibleBitmap(maindc,w,h);
        ///             ReleaseDC(GetDesktopWindow(),maindc);
        ///             oldbmp_image=(HBITMAP)SelectObject(dc_image,bmp_image);
        ///             oldbmp_thumbnail=(HBITMAP)SelectObject(dc_thumbnail,bmp_thumbnail);
        ///             if(is_nt) {
        ///                 SetStretchBltMode(dc_thumbnail,HALFTONE);
        ///                 SetBrushOrgEx(dc_thumbnail,0,0,&amp;pt);
        ///             } else {
        ///                 SetStretchBltMode(dc_thumbnail,COLORONCOLOR);
        ///             }
        ///             StretchBlt(dc_thumbnail,0,0,w,h,dc_image,0,0,bigx,bigy,SRCCOPY);
        ///             SelectObject(dc_image,oldbmp_image);
        ///             SelectObject(dc_thumbnail,oldbmp_thumbnail);
        ///             DeleteDC(dc_image);
        ///             DeleteDC(dc_thumbnail);
        ///             DeleteObject(bmp_image);
        ///             *ReturnedBitmap=bmp_thumbnail;
        ///             return FS_BITMAP_EXTRACTED | FS_BITMAP_CACHE;
        ///         }
        ///         *ReturnedBitmap=bmp_image;
        ///         return FS_BITMAP_EXTRACTED | FS_BITMAP_CACHE;
        ///     }
        ///     return FS_BITMAP_NONE;
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        PreviewBitmapResult GetPreviewBitmap(ref string remoteName, Size size, out Bitmap bitmap);


        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.IsLinksToLocalFiles"/> indicates that 
        /// the plugin is a temporary panel-style plugin or a normal file system plugin.
        /// Temporary file panels just hold links to files on the local file system.
        /// </summary>
        /// <returns>
        /// Return <strong>true</strong> if the plugin is a temporary panel-style plugin.
        /// Return <strong>false</strong> if the plugin is a normal file system plugin.
        /// </returns>
        /// <remarks>
        /// If your plugin is a temporary panel plugin, the following functions MUST be 
        /// thread-safe (can be called from background transfer manager):<br />
        /// - <see cref="ITotalCommanderWfxPlugin.IsLinksToLocalFiles"/><br />
        /// - <see cref="ITotalCommanderWfxPlugin.GetLocalName"/><br />
        /// - <see cref="ITotalCommanderWfxPlugin.FindFirst"/><br />
        /// - <see cref="ITotalCommanderWfxPlugin.FindNext"/><br />
        /// - <see cref="ITotalCommanderWfxPlugin.FindClose"/><br />
        /// This means that when uploading subdirectories from your plugin to FTP in the background, 
        /// Total Commander will call these functions in a background thread. If the user continues 
        /// to work in the foreground, calls to <see cref="ITotalCommanderWfxPlugin.FindFirst"/> and 
        /// <see cref="ITotalCommanderWfxPlugin.FindNext"/> may be occuring at the same 
        /// time! Therefore it's very important to use the search handle to keep temporary information 
        /// about the search. <see cref="ITotalCommanderWfxPlugin.StatusInfo"/> will NOT be called 
        /// from the background thread!
        /// </remarks>
        /// <seealso cref="ITotalCommanderWfxPlugin.IsLinksToLocalFiles"/><br />
        /// <seealso cref="ITotalCommanderWfxPlugin.GetLocalName"/><br />
        /// <seealso cref="ITotalCommanderWfxPlugin.FindFirst"/><br />
        /// <seealso cref="ITotalCommanderWfxPlugin.FindNext"/><br />
        /// <seealso cref="ITotalCommanderWfxPlugin.FindClose"/><br />
        /// <seealso cref="ITotalCommanderWfxPlugin.StatusInfo"/>
        bool IsLinksToLocalFiles();

        /// <summary>
        /// Temporary file panels just hold links to files on the local file system.
        /// </summary>
        /// <param name="remoteName">
        /// Full path to the file name in the plugin namespace, e.g. \somedir\file.ext.
        /// </param>
        /// <returns>
        /// Return the path of the file on the local file system, e.g. c:\windows\file.ext.
        /// </returns>
        /// <remarks>
        /// If your plugin is a temporary panel plugin, the following functions MUST be 
        /// thread-safe (can be called from background transfer manager):<br />
        /// - <see cref="ITotalCommanderWfxPlugin.IsLinksToLocalFiles"/><br />
        /// - <see cref="ITotalCommanderWfxPlugin.GetLocalName"/><br />
        /// - <see cref="ITotalCommanderWfxPlugin.FindFirst"/><br />
        /// - <see cref="ITotalCommanderWfxPlugin.FindNext"/><br />
        /// - <see cref="ITotalCommanderWfxPlugin.FindClose"/><br />
        /// This means that when uploading subdirectories from your plugin to FTP in the background, 
        /// Total Commander will call these functions in a background thread. If the user continues 
        /// to work in the foreground, calls to <see cref="ITotalCommanderWfxPlugin.FindFirst"/> and 
        /// <see cref="ITotalCommanderWfxPlugin.FindNext"/> may be occuring at the same 
        /// time! Therefore it's very important to use the search handle to keep temporary information 
        /// about the search. <see cref="ITotalCommanderWfxPlugin.StatusInfo"/> will NOT be called 
        /// from the background thread!
        /// </remarks>
        /// <seealso cref="ITotalCommanderWfxPlugin.IsLinksToLocalFiles"/><br />
        /// <seealso cref="ITotalCommanderWfxPlugin.GetLocalName"/><br />
        /// <seealso cref="ITotalCommanderWfxPlugin.FindFirst"/><br />
        /// <seealso cref="ITotalCommanderWfxPlugin.FindNext"/><br />
        /// <seealso cref="ITotalCommanderWfxPlugin.FindClose"/><br />
        /// <seealso cref="ITotalCommanderWfxPlugin.StatusInfo"/>
        string GetLocalName(string remoteName);


        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.StatusInfo"/> is called just as an information to the plugin that a 
        /// certain operation starts or ends. It can be used to allocate/free buffers, and/or to flush data from a cache. 
        /// </summary>
        /// <param name="remoteName">
        /// This is the current source directory when the operation starts. May be used to find out which part of the file system is affected.
        /// </param>
        /// <param name="origin">
        /// Information whether the operation starts or ends. Possible values:<br />
        /// <see cref="StatusOrigin.Start"/>: Operation starts (allocate buffers if needed)<br />
        /// <see cref="StatusOrigin.End"/>: Operation has ended (free buffers, flush cache etc)<br />
        /// </param>
        /// <param name="operation">
        /// Information of which operaration starts/ends. Possible values:<br />
        /// <list type="table">
        /// <item>
        /// <term><see cref="StatusOperation.List"/></term>
        /// <description>Retrieve a directory listing.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.GetSingle"/></term>
        /// <description>Get a single file from the plugin file system.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.GetMulti"/></term>
        /// <description>Get multiple files, may include subdirs.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.PutSingle"/></term>
        /// <description>Put a single file to the plugin file system.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.PutMulti"/></term>
        /// <description>Put multiple files, may include subdirs.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.RenameMoveSingle"/></term>
        /// <description>RenMov multiple files, may include subdirs.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.Delete"/></term>
        /// <description>Delete multiple files, may include subdirs.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.Attributes"/></term>
        /// <description>Change attributes/times, may include subdirs.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.CreateDirectory"/></term>
        /// <description>Create a single directory.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.Execute"/></term>
        /// <description>Start a single remote item, or a command line.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.CalculateSize"/></term>
        /// <description>Calculating size of subdir (user pressed SPACE).</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.Search"/></term>
        /// <description>Searching for file names only (using <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/>/<see cref="ITotalCommanderWfxPlugin.FindClose"/>).</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.SearchText"/></term>
        /// <description>Searching for file contents (using also <see cref="ITotalCommanderWfxPlugin.FileGet"/> calls).</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.SyncSearch"/></term>
        /// <description>Synchronize dirs searches subdirs for info.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.SyncGet"/></term>
        /// <description>Synchronize: Downloading files from plugin.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.SyncPut"/></term>
        /// <description>Synchronize: Uploading files to plugin.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.SyncDelete"/></term>
        /// <description>Synchronize: Deleting files from plugin.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.GetMultiThread"/></term>
        /// <description>Get multiple files, may include subdirs in background thread.</description>
        /// </item>
        /// <item>
        /// <term><see cref="StatusOperation.PutMultiThread"/></term>
        /// <description>Put multiple files, may include subdirs in background thread.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <remarks>
        /// <para>
        /// Please note that future versions of the framework may send additional values!
        /// </para>
        /// <para>
        /// This function has been added for the convenience of plugin writers. All calls to plugin functions will be enclosed in a pair of 
        /// <see cref="ITotalCommanderWfxPlugin.StatusInfo"/> calls. Multiple plugin calls can be between these two calls. 
        /// For example, a download may contain multiple calls to <see cref="ITotalCommanderWfxPlugin.FileGet"/>, and 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>, <see cref="ITotalCommanderWfxPlugin.FindNext"/>, 
        /// <see cref="ITotalCommanderWfxPlugin.FindClose"/> (for copying subdirs).
        /// </para>
        /// <para>
        /// Please also note that this function is only called for file operations. It isn't called for any of the <strong>Content*</strong> functions to 
        /// get file details, and also not for the following functions: 
        /// <see cref="ITotalCommanderWfxPlugin.SetDefaultParams"/>, <see cref="ITotalCommanderWfxPlugin.GetPreviewBitmap"/>, 
        /// <see cref="ITotalCommanderWfxPlugin.GetCustomIcon"/>, <see cref="ITotalCommanderWfxPlugin.Disconnect"/>.
        /// </para>
        /// </remarks>
        void StatusInfo(string remoteName, StatusOrigin origin, StatusOperation operation);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.Disconnect"/> is called when the user presses the Disconnect button in the FTP connections toolbar. 
        /// This toolbar is only shown if <see cref="Log.Connect"/> is called.
        /// </summary>
        /// <param name="disconnectRoot">This is the root dir which was passed to <see cref="Log.Connect"/> when connecting. 
        /// It allows the plugin to have serveral open connections to different file systems (e.g. ftp servers). 
        /// Should be either \ (for a single possible connection) or \Servername (e.g. when having multiple open connections).
        /// </param>
        /// <returns>Return <strong>true</strong> if the connection was closed (or never open), <strong>false</strong> if it couldn't be closed.</returns>
        /// <remarks>
        /// <para>
        /// To get calls to this function, the plugin MUST call <see cref="Log.Connect"/>. The parameter <paramref name="message"/> 
        /// MUST start with "CONNECT", followed by one whitespace and the root of the file system which has been connected. 
        /// This file system root will be passed to <see cref="ITotalCommanderWfxPlugin.Disconnect"/> when the user presses the Disconnect button, 
        /// so the plugin knows which connection to close.
        /// Do NOT call <see cref="Log.Connect"/> if your plugin does not require connect/disconnect!
        /// </para>
        /// <para>
        /// - FTP requires connect/disconnect. Connect can be done automatically when the user enters a subdir, 
        /// disconnect when the user clicks the Disconnect button.<br />
        /// - Access to local file systems (e.g. Linux EXT2) does not require connect/disconnect, so don't call <see cref="Log.Connect"/>.
        /// </para>
        /// </remarks>
        bool Disconnect(string disconnectRoot);

        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="error">The error.</param>
        void OnError(Exception error);
    }
}
