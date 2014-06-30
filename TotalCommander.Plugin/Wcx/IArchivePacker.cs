using System;
using System.Collections.Generic;

namespace TotalCommander.Plugin.Wcx
{
    public interface IArchivePacker : IDisposable
    {
        void PackFiles(string subPath, string sourcePath, string[] files, PackMode mode);

        void DeleteFiles(string[] files);

        bool PackInMemory(byte[] inbuffer, ref int taken, byte[] outbuffer, ref int written, int seekBy);
    }
}
