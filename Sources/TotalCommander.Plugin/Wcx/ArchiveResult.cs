
namespace TotalCommander.Plugin.Wcx
{
    public enum ArchiveResult
    {
        Success = 0,
        Done,
        EndArchive = 10,
        NoMemory,
        BadData,
        BadArchive,
        UnknownFormat,
        ErrorOpen,
        ErrorCreate,
        ErrorClose,
        ErrorRead,
        ErrorWrite,
        SmallBuffer,
        Aborted,
        NoFiles,
        TooManyFiles,
        NotSupported,
        Default = NotSupported,
    }
}
