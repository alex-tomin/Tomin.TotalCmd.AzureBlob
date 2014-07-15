using System;

namespace TotalCommander.Plugin.Wcx
{
    [Flags]
    public enum PackerCapabilities
    {
        None = 0,
        CanCreate = 1,
        CanModify = 2,
        Multiple = 4,
        CanDelete = 8,
        HasOptions = 16,
        CanPackInMemory = 32,
        CanDetectByContent = 64,
        CanSearchText = 128,
        HidePackerIcon = 256,
        CanEncrypt = 512,
    }
}
