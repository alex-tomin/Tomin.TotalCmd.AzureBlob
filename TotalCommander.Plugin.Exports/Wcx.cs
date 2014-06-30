using System;
using System.Runtime.InteropServices;
using System.Text;
using TotalCommander.Plugin.Wcx;

namespace TotalCommander.Plugin.Exports
{
    static class Wcx
    {
        private static ChangeVolProcW changeVolProc;
        private static ProcessDataProcW processDataProc;
        private static CryptProcW cryptProc;


        static Wcx()
        {
            AssemblyResolver.Init();
        }


        [DllExport]
        public static IntPtr OpenArchive(IntPtr archiveData)
        {
            return IntPtr.Zero;
        }

        [DllExport]
        public static IntPtr OpenArchiveW(IntPtr archiveData)
        {
            return WcxDispatcher.OpenArchive(archiveData);
        }

        [DllExport]
        public static int ReadHeader(IntPtr archive, IntPtr headerData)
        {
            return 0;
        }

        [DllExport]
        public static int ReadHeaderEx(IntPtr archive, IntPtr headerData)
        {
            return 0;
        }

        [DllExport]
        public static int ReadHeaderExW(IntPtr archive, IntPtr headerData)
        {
            return WcxDispatcher.ReadHeader(archive, headerData);
        }

        [DllExport]
        public static int ProcessFile(IntPtr archive, int operation, IntPtr path, IntPtr name)
        {
            return 0;
        }

        [DllExport]
        public static int ProcessFileW(IntPtr archive, int operation, IntPtr path, IntPtr name)
        {
            return WcxDispatcher.ProcessFile(archive, operation, path, name);
        }

        [DllExport]
        public static int CloseArchive(IntPtr archive)
        {
            return WcxDispatcher.CloseArchive(archive);
        }

        [DllExport]
        public static void SetChangeVolProc(IntPtr archive, [MarshalAs(UnmanagedType.FunctionPtr)] ChangeVolProcW callback)
        {
        }

        [DllExport]
        public static void SetChangeVolProcW(IntPtr archive, [MarshalAs(UnmanagedType.FunctionPtr)] ChangeVolProcW callback)
        {
            changeVolProc = callback;
            WcxDispatcher.SetChangeVolProcW(archive, ChangeVolume);
        }

        [DllExport]
        public static void SetProcessDataProc(IntPtr archive, [MarshalAs(UnmanagedType.FunctionPtr)] ProcessDataProcW callback)
        {
        }

        [DllExport]
        public static void SetProcessDataProcW(IntPtr archive, [MarshalAs(UnmanagedType.FunctionPtr)] ProcessDataProcW callback)
        {
            processDataProc = callback;
            WcxDispatcher.SetProcessDataProcW(archive, ProgressData);
        }


        [DllExport]
        public static int PackFiles(IntPtr packedFile, IntPtr subPath, IntPtr srcPath, IntPtr addList, int flags)
        {
            return 0;
        }

        [DllExport]
        public static int PackFilesW(
            [MarshalAs(UnmanagedType.LPWStr)] string packedFile,
            [MarshalAs(UnmanagedType.LPWStr)] string subPath,
            [MarshalAs(UnmanagedType.LPWStr)] string srcPath,
            IntPtr addList,
            Int32 flags)
        {
            return WcxDispatcher.PackFiles(packedFile, subPath, srcPath, addList, flags);
        }

        [DllExport]
        public static int DeleteFiles(IntPtr packedFile, IntPtr deleteList)
        {
            return 0;
        }

        [DllExport]
        public static int DeleteFilesW(
            [MarshalAs(UnmanagedType.LPWStr)] string packedFile,
            IntPtr deleteList)
        {
            return WcxDispatcher.DeleteFiles(packedFile, deleteList);
        }

        [DllExport]
        public static int GetPackerCaps()
        {
            return WcxDispatcher.GetPackerCaps();
        }

        [DllExport]
        public static void ConfigurePacker(IntPtr window, IntPtr dllInstance)
        {
            WcxDispatcher.ConfigurePacker(window, dllInstance);
        }

        [DllExport]
        public static int StartMemPack(int options, IntPtr fileName)
        {
            return 0;
        }

        [DllExport]
        public static IntPtr StartMemPackW(int options, IntPtr fileName)
        {
            return WcxDispatcher.StartMemPack(options, fileName);
        }

        [DllExport]
        public static int PackToMem(
            IntPtr hMemPack, 
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] bufIn, 
            int inLen, 
            ref Int32 taken,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] byte[] bufOut, 
            int outLen, 
            ref Int32 written, 
            int seekBy)
        {
            return WcxDispatcher.PackToMem(hMemPack, bufIn, ref taken, bufOut, ref written, seekBy);
        }

        [DllExport]
        public static int DoneMemPack(IntPtr hMemPack)
        {
            return WcxDispatcher.DoneMemPack(hMemPack);
        }

        [DllExport]
        public static bool CanYouHandleThisFile(IntPtr fileName)
        {
            return false;
        }

        [DllExport]
        public static bool CanYouHandleThisFileW(IntPtr fileName)
        {
            return WcxDispatcher.CanYouHandleThisFile(fileName);
        }

        [DllExport]
        public static void PackSetDefaultParams(IntPtr dps)
        {
            WcxDispatcher.PackSetDefaultParams(dps);
        }

        [DllExport]
        public static void PkSetCryptCallback(CryptProcW callback, int number, int flags)
        {
            cryptProc = callback;
            WcxDispatcher.SetCryptCallbackW(Crypt, number, flags);
        }

        [DllExport]
        public static int GetBackgroundFlags()
        {
            return WcxDispatcher.GetBackgroundFlags();
        }


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate Int32 ChangeVolProcW(
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder arcName,
            Int32 mode
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate Int32 ProcessDataProcW(
            [MarshalAs(UnmanagedType.LPWStr)] string fileName,
            Int32 size
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate Int32 CryptProcW(
            Int32 cryptoNumber,
            Int32 mode,
            [MarshalAs(UnmanagedType.LPWStr)] string archiveName,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder password,
            Int32 maxLen
        );


        private static int ChangeVolume(StringBuilder arcName, int mode)
        {
            return changeVolProc(arcName, mode);
        }

        private static int ProgressData(string filename, int size)
        {
            return processDataProc(filename, size);
        }

        private static int Crypt(int cryptoNumber, int mode, string connectionName, StringBuilder password, int maxLen)
        {
            return cryptProc(cryptoNumber, mode, connectionName, password, maxLen);
        }
    }
}
