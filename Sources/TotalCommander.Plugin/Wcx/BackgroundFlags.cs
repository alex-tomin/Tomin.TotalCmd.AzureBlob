using System;

namespace TotalCommander.Plugin.Wcx
{
    [Flags]
    public enum BackgroundFlags
    {
        NotSupported = 0,
        Unpack = 1,
        Pack = 2,
        MemoryPack = 4,
    }
}
