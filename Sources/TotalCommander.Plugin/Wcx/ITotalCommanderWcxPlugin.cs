using System;

namespace TotalCommander.Plugin.Wcx
{
    public interface ITotalCommanderWcxPlugin
    {
        void SetDefaultParams(DefaultParam dp);

        void SetPassword(Password password);

        BackgroundFlags GetBackgroundFlags();

        PackerCapabilities GetPackerCapabilities();

        bool CanHandleThisFile(string fileName);

        
        ArchiveResult OpenArchive(string archiveName, OpenArchiveMode mode, out IntPtr archive);

        void SetChangeVolume(IntPtr archive, ChangeVolume changeVolume);

        void SetProgress(IntPtr archive, Progress progress);

        ArchiveResult ReadHeader(IntPtr archive, out ArchiveHeader header);

        ArchiveResult ProcessFile(IntPtr archive, ArchiveProcess operation, string filepath, string filename);

        ArchiveResult CloseArchive(IntPtr archive);


        void ConfigurePacker(IntPtr window, IntPtr dllInstance);

        ArchiveResult PackFiles(string archiveName, string subPath, string sourcePath, string[] addList, PackMode mode);

        ArchiveResult DeleteFiles(string archiveName, string[] deleteList);


        IntPtr StartMemoryPack(MemoryPackMode options, string fileName);

        ArchiveResult PackToMemory(IntPtr hMemPack, byte[] bufIn, ref Int32 taken, byte[] bufOut, ref Int32 written, int seekBy);

        ArchiveResult DoneMemoryPack(IntPtr hMemPack);
        
        
        void UnhandledException(Exception ex);
    }
}
