using System;
using System.Diagnostics;

namespace TotalCommander.Plugin.Wcx
{
    static class WcxDispatcher
    {
        private static ITotalCommanderWcxPlugin Plugin
        {
            get { return TotalCommanderPluginHolder.GetWcxPlugin(); }
        }


        public static void PackSetDefaultParams(IntPtr dps)
        {
            try
            {
                Plugin.SetDefaultParams(new DefaultParam(dps));
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
        }

        public static int GetPackerCaps()
        {
            var capabilities = PackerCapabilities.None;
            try
            {
                capabilities = Plugin.GetPackerCapabilities();
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return (int)capabilities;
        }

        public static int GetBackgroundFlags()
        {
            var flags = BackgroundFlags.NotSupported;
            try
            {
                flags = Plugin.GetBackgroundFlags();
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return (int)flags;
        }

        public static void ConfigurePacker(IntPtr window, IntPtr dllInstance)
        {
            try
            {
                Plugin.ConfigurePacker(window, dllInstance);
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
        }


        public static void SetChangeVolProcW(IntPtr archive, ChangeVolume.Callback callback)
        {
            try
            {
                Plugin.SetChangeVolume(archive, new ChangeVolume(callback));
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
        }

        public static void SetProcessDataProcW(IntPtr archive, Progress.Callback callback)
        {
            try
            {
                Plugin.SetProgress(archive, new Progress(callback));
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
        }

        public static void SetCryptCallbackW(Password.Callback callback, int number, int flags)
        {
            try
            {
                Plugin.SetPassword(new Password(callback, number, flags));
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
        }


        public static IntPtr OpenArchive(IntPtr archiveData)
        {
            var info = new OpenArchiveInfo(archiveData);
            var archive = IntPtr.Zero;
            try
            {
                info.Result = Plugin.OpenArchive(info.ArchiveName, info.Mode, out archive);
            }
            catch (WcxException error)
            {
                info.Result = error.ArchiveResult;
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return archive;
        }

        public static int ReadHeader(IntPtr archive, IntPtr headerData)
        {
            var result = ArchiveResult.Default;
            try
            {
                ArchiveHeader header;
                result = Plugin.ReadHeader(archive, out header);
                if (header != null) header.CopyTo(headerData);
            }
            catch (WcxException error)
            {
                result = error.ArchiveResult;
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return (int)result;
        }

        public static int ProcessFile(IntPtr archive, int operation, IntPtr path, IntPtr name)
        {
            var result = ArchiveResult.Default;
            try
            {
                result = Plugin.ProcessFile(archive, (ArchiveProcess)operation, Win32.GetString(path), Win32.GetString(name));
            }
            catch (WcxException error)
            {
                result = error.ArchiveResult;
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return (int)result;
        }

        public static int CloseArchive(IntPtr archive)
        {
            var result = ArchiveResult.Default;
            try
            {
                result = Plugin.CloseArchive(archive);
            }
            catch (WcxException error)
            {
                result = error.ArchiveResult;
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return (int)result;
        }


        public static int PackFiles(string packedFile, string subPath, string srcPath, IntPtr addList, int flags)
        {
            var result = ArchiveResult.Default;
            try
            {
                result = Plugin.PackFiles(packedFile, subPath, srcPath, Win32.GetStringArray(addList), (PackMode)flags);
            }
            catch (WcxException error)
            {
                result = error.ArchiveResult;
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return (int)result;
        }

        public static int DeleteFiles(string packedFile, IntPtr deleteList)
        {
            var result = ArchiveResult.Default;
            try
            {
                result = Plugin.DeleteFiles(packedFile, Win32.GetStringArray(deleteList));
            }
            catch (WcxException error)
            {
                result = error.ArchiveResult;
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return (int)result;
        }

        public static IntPtr StartMemPack(int options, IntPtr fileName)
        {
            var result = IntPtr.Zero;
            try
            {
                result = Plugin.StartMemoryPack((MemoryPackMode)options, Win32.GetString(fileName));
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return result;
        }

        public static int PackToMem(IntPtr hMemPack, byte[] bufIn, ref Int32 taken, byte[] bufOut, ref Int32 written, int seekBy)
        {
            var result = ArchiveResult.Default;
            try
            {
                result = Plugin.PackToMemory(hMemPack, bufIn, ref taken, bufOut, ref written, seekBy);
            }
            catch (WcxException error)
            {
                result = error.ArchiveResult;
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return (int)result;
        }

        public static int DoneMemPack(IntPtr hMemPack)
        {
            var result = ArchiveResult.Default;
            try
            {
                result = Plugin.DoneMemoryPack(hMemPack);
            }
            catch (WcxException error)
            {
                result = error.ArchiveResult;
            }
            catch (Exception ex)
            {
                ProcessUnhandledException(ex);
            }
            return (int)result;
        }


        public static bool CanYouHandleThisFile(IntPtr fileName)
        {
            return Plugin.CanHandleThisFile(Win32.GetString(fileName));
        }


        private static void ProcessUnhandledException(Exception ex)
        {
            Plugin.UnhandledException(ex);
        }
    }
}
