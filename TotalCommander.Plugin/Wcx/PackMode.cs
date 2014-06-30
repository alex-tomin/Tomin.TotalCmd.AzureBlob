using System;

namespace TotalCommander.Plugin.Wcx
{
    [Flags]
    public enum PackMode
    {
        None = 0,
        MoveFiles = 1,
        SavePaths = 2,
        Encrypt = 4,
    }
}
