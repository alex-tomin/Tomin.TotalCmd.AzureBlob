using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using TotalCommander.Plugin.Utils;

namespace TotalCommander.Plugin.Wfx
{
    static class WfxDispatcher
    {
        private static ITotalCommanderWfxPlugin Plugin
        {
            get { return TotalCommanderPluginHolder.GetWfxPlugin(); }
        }

        private static IDictionary<IntPtr, IEnumerator> enumerators = new Dictionary<IntPtr, IEnumerator>();

        private static int pluginNumber;


        public static Int32 FsInit(Int32 number, Progress.Callback progress, Log.Callback log, Request.Callback request)
        {
            try
            {
                pluginNumber = number;
                Plugin.Init(
                    number,
                    new Progress(number, progress),
                    new Log(number, log),
                    new Request(number, Plugin.PluginName, request)
                );
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return 0;
        }

        public static IntPtr FsFindFirst(string path, IntPtr pFindData)
        {
            var handle = Win32.INVALID_HANDLE;
            try
            {
                IEnumerator enumerator = null;
                var findData = Plugin.FindFirst(path, out enumerator);
                if (findData != null)
                {
                    if (findData == FindData.NoMoreFiles)
                    {
                        Win32.SetLastError(Win32.ERROR_NO_MORE_FILES);
                    }
                    else
                    {
                        findData.CopyTo(pFindData);
                        lock (enumerators)
                        {
                            handle = new IntPtr(enumerators.Count + 1);
                            enumerators[handle] = enumerator;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return handle;
        }

        public static bool FsFindNext(IntPtr handle, IntPtr pFindData)
        {
            try
            {
                IEnumerator enumerator = null;
                lock (enumerators)
                {
                    enumerators.TryGetValue(handle, out enumerator);
                }
                var findData = Plugin.FindNext(enumerator);
                if (findData != null && findData != FindData.NoMoreFiles)
                {
                    findData.CopyTo(pFindData);
                    return true;
                }
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return false;
        }

        public static Int32 FsFindClose(IntPtr handle)
        {
            try
            {
                IEnumerator enumerator = null;
                lock (enumerators)
                {
                    if (enumerators.TryGetValue(handle, out enumerator))
                    {
                        enumerators.Remove(handle);
                    }
                }
                Plugin.FindClose(enumerator);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return 0;
        }

        public static void FsSetDefaultParams(IntPtr dps)
        {
            try
            {
                Plugin.SetDefaultParams(new DefaultParam(dps));
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
        }

        public static string FsGetDefRootName()
        {
            try
            {
                return Plugin.PluginName;
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return null;
        }

        public static int FsExecuteFile(IntPtr mainWin, IntPtr remoteName, string verb)
        {
            var result = ExecuteResult.Default;
            try
            {
                var nameRef = Win32.GetString(remoteName);
                if (!string.IsNullOrEmpty(nameRef))
                {
                    result = Plugin.FileExecute(new TotalCommanderWindow(mainWin), ref nameRef, verb);
                    Win32.SetString(remoteName, nameRef);
                }
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return (int)result;
        }

        public static int FsGetFile(string remoteName, IntPtr localName, int copyFlags, IntPtr ri)
        {
            var result = FileOperationResult.Default;
            try
            {
                var nameRef = Win32.GetString(localName);
                result = Plugin.FileGet(remoteName, ref nameRef, (CopyFlags)copyFlags, new RemoteInfo(ri));
                Win32.SetString(localName, nameRef);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return (int)result;
        }

        public static int FsPutFile(string localName, IntPtr remoteName, int copyFlags)
        {
            var result = FileOperationResult.Default;
            try
            {
                var nameRef = Win32.GetString(remoteName);
                result = Plugin.FilePut(localName, ref nameRef, (CopyFlags)copyFlags);
                Win32.SetString(remoteName, nameRef);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return (int)result;
        }

        public static int FsRenMovFile(string oldName, string newName, bool move, bool overWrite, IntPtr ri)
        {
            var result = FileOperationResult.Default;
            try
            {
                result = Plugin.FileRenameMove(oldName, newName, move, overWrite, new RemoteInfo(ri));
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return (int)result;
        }

        public static bool FsDeleteFile(string remoteName)
        {
            var result = false;
            try
            {
                result = Plugin.FileRemove(remoteName);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return result;
        }

        public static bool FsMkDir(string path)	
        {
            var result = false;
            try
            {
                result = Plugin.DirectoryCreate(path);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return result;
        }

        public static bool FsRemoveDir(string remoteName)
        {
            var result = false;
            try
            {
                result = Plugin.DirectoryRemove(remoteName);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return result;
        }

        public static bool FsSetAttr(string remoteName, int newAttr)
        {
            var result = false;
            try
            {
                result = Plugin.SetFileAttributes(remoteName, (FileAttributes)newAttr);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return result;
        }

        public static bool FsSetTime(string remoteName, FILETIME creationTime, FILETIME lastAccessTime, FILETIME lastWriteTime)
        {
            var result = false;
            try
            {
                result = Plugin.SetFileTime(
                    remoteName,
                    DateTimeUtil.FromFileTime(creationTime),
                    DateTimeUtil.FromFileTime(lastAccessTime),
                    DateTimeUtil.FromFileTime(lastWriteTime)
                );
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return result;
        }

        public static bool FsDisconnect(string disconnectRoot)
        {
            var result = false;
            try
            {
                result = Plugin.Disconnect(disconnectRoot);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return result;
        }

        public static void FsStatusInfo(string remoteDir, int infoStartEnd, int infoOperation)
        {
            try
            {
                Plugin.StatusInfo(remoteDir, (StatusOrigin)infoStartEnd, (StatusOperation)infoOperation);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
        }

        public static int FsExtractCustomIcon(IntPtr remoteName, int extractFlags, ref IntPtr iconHandle)
        {
            var result = CustomIconResult.UseDefault;
            try
            {
                Icon icon = null;
                var nameRef = Win32.GetString(remoteName);
                result = Plugin.GetCustomIcon(ref nameRef, (CustomIconFlags)extractFlags, out icon);
                if (icon != null) iconHandle = icon.Handle;
                Win32.SetString(remoteName, nameRef);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return (int)result;
        }

        public static int FsGetPreviewBitmap(IntPtr remoteName, int width, int height, ref IntPtr bitmapHandle)
        {
            var result = PreviewBitmapResult.None;
            try
            {
                Bitmap bitmap = null;
                var nameRef = Win32.GetString(remoteName);
                result = Plugin.GetPreviewBitmap(ref nameRef, new Size(width, height), out bitmap);
                if (bitmap != null) bitmapHandle = bitmap.GetHbitmap();
                Win32.SetString(remoteName, nameRef);
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return (int)result;
        }

        public static void FsSetCryptCallback(Password.Callback password, int cryptoNumber, int flags)
        {
            try
            {
                Plugin.SetPasswordStore(new Password(password, pluginNumber, cryptoNumber, flags));
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
        }

        public static bool FsLinksToLocalFiles()
        {
            var result = false;
            try
            {
                result = Plugin.IsLinksToLocalFiles();
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return result;
        }

        public static bool FsGetLocalName(IntPtr ptr, int maxlen)
        {
            var result = false;
            try
            {
                var remote = Win32.GetString(ptr);
                var local = Plugin.GetLocalName(remote);
                if (!string.IsNullOrEmpty(local) && local != remote)
                {
                    Win32.SetString(ptr, local, maxlen, Encoding.Unicode);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return result;
        }

        public static int FsGetBackgroundFlags()
        {
            var result = 0;
            try
            {
                result = (int)Plugin.GetBackgroundFlags();
            }
            catch (Exception ex)
            {
                UnhandledError(ex);
            }
            return result;
        }


        private static void UnhandledError(Exception ex)
        {
            try
            {
                if (Plugin != null)
                {
                    Plugin.OnError(ex);
                }
            }
            catch { }
        }
    }
}
