using System;
using System.Runtime.InteropServices;
using System.Text;
using TotalCommander.Plugin.Wfx;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TotalCommander.Plugin.Exports
{
    static class Wfx
    {
        private static ProgressProcW progressProc;
        private static LogProcW logProc;
        private static RequestProcW requestProc;
        private static CryptProcW cryptProc;


        static Wfx()
        {
            AssemblyResolver.Init();
        }


        [DllExport]
        public static void FsGetDefRootName(IntPtr ptr, Int32 maxLen)
        {
            var pluginName = WfxDispatcher.FsGetDefRootName();
            var ansi = Encoding.ASCII.GetString(Encoding.Convert(Encoding.Unicode, Encoding.ASCII, Encoding.Unicode.GetBytes(pluginName)));
            Win32.SetString(ptr, ansi, maxLen, Encoding.ASCII);
        }

        [DllExport]
        public static Int32 FsInitW(
            Int32 pluginNumber,
            [MarshalAs(UnmanagedType.FunctionPtr)] ProgressProcW pProgressProc,
            [MarshalAs(UnmanagedType.FunctionPtr)] LogProcW pLogProc,
            [MarshalAs(UnmanagedType.FunctionPtr)] RequestProcW pRequestProc)
        {
            progressProc = pProgressProc;
            logProc = pLogProc;
            requestProc = pRequestProc;
            return WfxDispatcher.FsInit(pluginNumber, Progress, Log, Request);
        }

        [DllExport]
        public static IntPtr FsFindFirstW([MarshalAs(UnmanagedType.LPWStr)]string path, IntPtr pFindData)
        {
            return WfxDispatcher.FsFindFirst(path, pFindData);
        }

        [DllExport]
        public static bool FsFindNextW(IntPtr handle, IntPtr pFindData)
        {
            return WfxDispatcher.FsFindNext(handle, pFindData);
        }

        [DllExport]
        public static Int32 FsFindClose(IntPtr handle)
        {
            return WfxDispatcher.FsFindClose(handle);
        }

        [DllExport]
        public static void FsSetDefaultParams(IntPtr dps)
        {
            WfxDispatcher.FsSetDefaultParams(dps);
        }


        [DllExport]
        public static Int32 FsExecuteFileW(
            IntPtr mainWin,
            IntPtr remoteName,
            [MarshalAs(UnmanagedType.LPWStr)] string verb)
        {
            return WfxDispatcher.FsExecuteFile(mainWin, remoteName, verb);
        }

        [DllExport]
        public static int FsRenMovFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string oldName,
            [MarshalAs(UnmanagedType.LPWStr)] string newName,
            [MarshalAs(UnmanagedType.Bool)] bool move,
            [MarshalAs(UnmanagedType.Bool)] bool overWrite,
            IntPtr ri)
        {
            return WfxDispatcher.FsRenMovFile(oldName, newName, move, overWrite, ri);
        }

        [DllExport]
        public static bool FsDeleteFileW([MarshalAs(UnmanagedType.LPWStr)] string remoteName)
        {
            return WfxDispatcher.FsDeleteFile(remoteName);
        }

        [DllExport]
        public static bool FsMkDirW([MarshalAs(UnmanagedType.LPWStr)] string path)
        {
            return WfxDispatcher.FsMkDir(path);
        }

        [DllExport]
        public static bool FsRemoveDirW([MarshalAs(UnmanagedType.LPWStr)] string remoteName)
        {
            return WfxDispatcher.FsRemoveDir(remoteName);
        }

        [DllExport]
        public static Int32 FsGetFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string remoteName,
            IntPtr localName,
            Int32 copyFlags,
            IntPtr ri)
        {
            return WfxDispatcher.FsGetFile(remoteName, localName, copyFlags, ri);
        }

        [DllExport]
        public static Int32 FsPutFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string localName,
            IntPtr remoteName,
            Int32 copyFlags)
        {
            return WfxDispatcher.FsPutFile(localName, remoteName, copyFlags);
        }

        [DllExport]
        public static bool FsSetAttrW(
            [MarshalAs(UnmanagedType.LPWStr)] string remoteName,
            Int32 newAttr)
        {
            return WfxDispatcher.FsSetAttr(remoteName, newAttr);
        }

        [DllExport]
        public static bool FsSetTimeW(
            [MarshalAs(UnmanagedType.LPWStr)] string remoteName,
            [MarshalAs(UnmanagedType.LPStruct)] FileTime creationTime,
            [MarshalAs(UnmanagedType.LPStruct)] FileTime lastAccessTime,
            [MarshalAs(UnmanagedType.LPStruct)] FileTime lastWriteTime)
        {
            return WfxDispatcher.FsSetTime(remoteName, creationTime, lastAccessTime, lastWriteTime);
        }

        [DllExport]
        public static bool FsDisconnectW([MarshalAs(UnmanagedType.LPWStr)] string disconnectRoot)
        {
            return WfxDispatcher.FsDisconnect(disconnectRoot);
        }

        [DllExport]
        public static void FsStatusInfoW(
            [MarshalAs(UnmanagedType.LPWStr)] string remoteDir,
            Int32 infoStartEnd,
            Int32 infoOperation)
        {
            WfxDispatcher.FsStatusInfo(remoteDir, infoStartEnd, infoOperation);
        }

        [DllExport]
        public static Int32 FsExtractCustomIconW(
            IntPtr remoteName,
            int extractFlags,
            ref IntPtr theIcon)
        {
            return WfxDispatcher.FsExtractCustomIcon(remoteName, extractFlags, ref theIcon);
        }

        [DllExport]
        public static Int32 FsGetPreviewBitmapW(
            IntPtr remoteName,
            Int32 width,
            Int32 height,
            ref IntPtr returnedBitmap)
        {
            return WfxDispatcher.FsGetPreviewBitmap(remoteName, width, height, ref returnedBitmap);
        }

        [DllExport]
        public static void FsSetCryptCallbackW(
            [MarshalAs(UnmanagedType.FunctionPtr)] CryptProcW pCryptProc,
            Int32 cryptoNumber,
            Int32 flags)
        {
            cryptProc = pCryptProc;
            WfxDispatcher.FsSetCryptCallback(Crypt, cryptoNumber, flags);
        }

        [DllExport]
        public static bool FsLinksToLocalFiles()
        {
            return WfxDispatcher.FsLinksToLocalFiles();
        }

        [DllExport]
        public static bool FsGetLocalNameW(IntPtr remoteName, Int32 maxlen)
        {
            return WfxDispatcher.FsGetLocalName(remoteName, maxlen);
        }

        [DllExport]
        public static Int32 FsGetBackgroundFlags()
        {
            return WfxDispatcher.FsGetBackgroundFlags();
        }


        #region Private Methods

        private static int Progress(int pluginNumber, string sourceName, string targetName, int percentDone)
        {
            return progressProc(pluginNumber, sourceName, targetName, percentDone);
        }

        private static void Log(int pluginNumber, int messageType, string logString)
        {
            logProc(pluginNumber, messageType, logString);
        }

        private static bool Request(int pluginNumber, int requestType, string customTitle, string customText, StringBuilder defaultText, int maxLen)
        {
            return requestProc(pluginNumber, requestType, customTitle, customText, defaultText, maxLen);
        }

        private static int Crypt(int pluginNumber, int cryptoNumber, int mode, string connectionName, StringBuilder password, int maxLen)
        {
            return cryptProc(pluginNumber, cryptoNumber, mode, connectionName, password, maxLen);
        }

        #endregion


        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate void LogProcW(
            Int32 pluginNumber,
            Int32 messageType,
            [MarshalAs(UnmanagedType.LPWStr)] string logString
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int ProgressProcW(
            Int32 pluginNumber,
            [MarshalAs(UnmanagedType.LPWStr)] string sourceName,
            [MarshalAs(UnmanagedType.LPWStr)] string targetName,
            Int32 percentDone
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate bool RequestProcW(
            Int32 pluginNumber,
            Int32 requestType,
            [MarshalAs(UnmanagedType.LPWStr)] string customTitle,
            [MarshalAs(UnmanagedType.LPWStr)] string customText,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder defaultText,
            Int32 maxLen
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate Int32 CryptProcW(
            Int32 pluginNumber,
            Int32 cryptoNumber,
            Int32 mode,
            [MarshalAs(UnmanagedType.LPWStr)] string connectionName,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder password,
            Int32 maxLen
        );

        #endregion


        #region ANSI Implementation

        [DllExport]
        public static Int32 FsInit(Int32 pluginNumber, IntPtr pProgressProc, IntPtr pLogProc, IntPtr pRequestProc)
        {
            return 0;
        }

        [DllExport]
        public static IntPtr FsFindFirst(IntPtr path, IntPtr pFindData)
        {
            return IntPtr.Zero;
        }

        [DllExport]
        public static bool FsFindNext(IntPtr handle, IntPtr pFindData)
        {
            return false;
        }

        [DllExport]
        public static Int32 FsExecuteFile(IntPtr mainWin, IntPtr remoteName, IntPtr verb)
        {
            return 0;
        }

        [DllExport]
        public static int FsRenMovFile(IntPtr oldName, IntPtr newName, bool move, bool overWrite, IntPtr ri)
        {
            return 0;
        }

        [DllExport]
        public static bool FsDeleteFile(IntPtr remoteName)
        {
            return false;
        }

        [DllExport]
        public static bool FsMkDir(IntPtr path)
        {
            return false;
        }

        [DllExport]
        public static bool FsRemoveDir(IntPtr remoteName)
        {
            return false;
        }

        [DllExport]
        public static Int32 FsGetFile(IntPtr remoteName, IntPtr localName, Int32 copyFlags, IntPtr ri)
        {
            return 0;
        }

        [DllExport]
        public static Int32 FsPutFile(IntPtr localName, IntPtr remoteName, Int32 copyFlags)
        {
            return 0;
        }

        [DllExport]
        public static bool FsSetAttr(IntPtr remoteName, Int32 newAttr)
        {
            return false;
        }

        [DllExport]
        public static bool FsSetTime(IntPtr remoteName, [MarshalAs(UnmanagedType.LPStruct)] FileTime creationTime, [MarshalAs(UnmanagedType.LPStruct)] FileTime lastAccessTime, [MarshalAs(UnmanagedType.LPStruct)] FileTime lastWriteTime)
        {
            return false;
        }

        [DllExport]
        public static bool FsDisconnect(IntPtr disconnectRoot)
        {
            return false;
        }

        [DllExport]
        public static void FsStatusInfo(IntPtr remoteDir, Int32 infoStartEnd, Int32 infoOperation)
        {
        }

        [DllExport]
        public static Int32 FsExtractCustomIcon(IntPtr remoteName, int extractFlags, ref IntPtr theIcon)
        {
            return 0;
        }

        [DllExport]
        public static Int32 FsGetPreviewBitmap(IntPtr remoteName, Int32 width, Int32 height, ref IntPtr returnedBitmap)
        {
            return 0;
        }

        [DllExport]
        public static void FsSetCryptCallback(IntPtr pCryptProc, Int32 cryptoNumber, Int32 flags)
        {
        }

        [DllExport]
        public static bool FsGetLocalName(IntPtr remoteName, Int32 maxlen)
        {
            return false;
        }

        #endregion
    }
}
