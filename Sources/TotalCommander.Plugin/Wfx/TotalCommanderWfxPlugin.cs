using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace TotalCommander.Plugin.Wfx
{
    public abstract class TotalCommanderWfxPlugin : ITotalCommanderWfxPlugin
    {
        /// <summary>
        /// The default root name which should appear in the Network Neighborhood.
        /// </summary>
        public abstract string PluginName
        {
            get;
        }

        /// <summary>
        /// Internal number this plugin was given in Total Commander.
        /// </summary>
        public int PluginNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// <see cref="Progress"/> class, which contains progress functions.
        /// </summary>
        public Progress Progress
        {
            get;
            private set;
        }

        /// <summary>
        /// <see cref="Log"/> class, which contains logging functions.
        /// </summary>
        public Log Log
        {
            get;
            private set;
        }

        /// <summary>
        /// <see cref="Request"/> class, which contains request text functions.
        /// </summary>
        public Request Request
        {
            get;
            private set;
        }

        /// <summary>
        /// You can use this class to store passwords in Total Commander's secure password store.
        /// </summary>
        public Password Password
        {
            get;
            private set;
        }

        /// <summary>
        /// Plugin interface version.
        /// </summary>
        public Version PluginInterfaceVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Suggested location+name of the ini file where the plugin could store its data.
        /// </summary>
        public string PluginIniFile
        {
            get;
            private set;
        }

        /// <summary>
        /// The plugin is a temporary panel-style plugin or a normal file system plugin.
        /// Temporary file panels just hold links to files on the local file.
        /// </summary>
        public virtual bool TemporaryPanelPlugin
        {
            get { return false; }
        }

        /// <summary>
        /// Called by Total Commander 7.51 or newer to determine whether the plugin supports 
        /// background operations (uploads and downloads), and if yes, how they are supported.
        /// </summary>
        public virtual BackgroundFlags BackgroundSupport
        {
            get { return BackgroundFlags.NotSupported; }
        }


        void ITotalCommanderWfxPlugin.Init(int pluginNumber, Progress progress, Log log, Request request)
        {
            PluginNumber = pluginNumber;
            Progress = progress;
            Log = log;
            Request = request;

            Initialize();
        }

        /// <summary>
        /// Called when loading the plugin.
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// Сalled to retrieve the first file in a directory of the plugin's file system.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public abstract FindData FindFirst(string path, out IEnumerator enumerator);

        /// <summary>
        /// Сalled to retrieve the next file in a directory of the plugin's file system.
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public virtual FindData FindNext(IEnumerator enumerator)
        {
            return FindData.NoMoreFiles;
        }

        /// <summary>
        /// Сalled to end a FindFirst/FindNext loop, either after retrieving all files, or when the user aborts it.
        /// </summary>
        /// <param name="enumerator"></param>
        public virtual void FindClose(IEnumerator enumerator)
        {

        }


        ExecuteResult ITotalCommanderWfxPlugin.FileExecute(TotalCommanderWindow window, ref string remoteName, string verb)
        {
            if (string.IsNullOrEmpty(verb)) return ExecuteResult.Error;
            var command = 0 < verb.IndexOf(' ') ? verb.Substring(verb.IndexOf(' ')).Trim() : string.Empty;
            switch (verb.ToLower().Substring(0, 1))
            {
                case "o": return ExecuteOpen(window, ref remoteName);
                case "p": return ExecuteProperties(window, ref remoteName);
                case "c": return ExecuteChMode(window, ref remoteName, command);
                case "q": return ExecuteCommand(window, ref remoteName, command);
            }
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult ExecuteProperties(TotalCommanderWindow window, ref string remoteName)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult ExecuteChMode(TotalCommanderWindow window, ref string remoteName, string mode)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult ExecuteCommand(TotalCommanderWindow window, ref string remoteName, string command)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult ExecuteOpen(TotalCommanderWindow window, ref string remoteName)
        {
            return ExecuteResult.Default;
        }


        public virtual FileOperationResult FileGet(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri)
        {
            return FileOperationResult.Default;
        }

        public virtual FileOperationResult FilePut(string localName, ref string remoteName, CopyFlags copyFlags)
        {
            return FileOperationResult.Default;
        }

        FileOperationResult ITotalCommanderWfxPlugin.FileRenameMove(string oldName, string newName, bool move, bool overwrite, RemoteInfo ri)
        {
            return ri.IsDirectory ?
                DirectoryRename(oldName, newName, overwrite, ri) :
                FileCopy(oldName, newName, overwrite, move, ri);
        }

        public virtual FileOperationResult FileCopy(string source, string target, bool overwrite, bool move, RemoteInfo ri)
        {
            return FileOperationResult.Default;
        }

        public virtual FileOperationResult DirectoryRename(string oldName, string newName, bool overwrite, RemoteInfo ri)
        {
            return FileOperationResult.Default;
        }


        public virtual bool FileRemove(string remoteName)
        {
            return false;
        }

        public virtual bool DirectoryCreate(string remoteName)
        {
            return false;
        }

        public virtual bool DirectoryRemove(string remoteName)
        {
            return false;
        }

        BackgroundFlags ITotalCommanderWfxPlugin.GetBackgroundFlags()
        {
            return BackgroundSupport;
        }


        public virtual CustomIconResult GetCustomIcon(ref string remoteName, CustomIconFlags extractIconFlag, out Icon icon)
        {
            icon = null;
            return CustomIconResult.UseDefault;
        }

        public virtual PreviewBitmapResult GetPreviewBitmap(ref string remoteName, Size size, out Bitmap bitmap)
        {
            bitmap = null;
            return PreviewBitmapResult.None;
        }


        bool ITotalCommanderWfxPlugin.IsLinksToLocalFiles()
        {
            return TemporaryPanelPlugin;
        }

        public virtual string GetLocalName(string remoteName)
        {
            return null;
        }


        void ITotalCommanderWfxPlugin.SetDefaultParams(DefaultParam defaultParam)
        {
            PluginInterfaceVersion = defaultParam.PluginInterfaceVersion;
            PluginIniFile = defaultParam.DefaultIniFileName;
        }

        public virtual bool SetFileAttributes(string remoteName, FileAttributes attributes)
        {
            return false;
        }

        public virtual bool SetFileTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime)
        {
            return false;
        }

        void ITotalCommanderWfxPlugin.SetPasswordStore(Password password)
        {
            Password = password;
        }


        public virtual bool Disconnect(string disconnectRoot)
        {
            return false;
        }

        public virtual void StatusInfo(string remoteName, StatusOrigin origin, StatusOperation operation)
        {

        }

        public virtual void OnError(Exception error)
        {

        }
    }
}
