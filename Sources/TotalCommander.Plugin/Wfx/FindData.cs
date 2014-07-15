using System;
using System.IO;
using System.Runtime.InteropServices;
using TotalCommander.Plugin.Utils;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// <see cref="FindData"/> is used by the
    /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/> and <see cref="ITotalCommanderWfxPlugin.FindNext"/> methods.
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.FindFirst"/>
    /// <seealso cref="ITotalCommanderWfxPlugin.FindNext"/>
    public class FindData
    {
        /// <summary>
        /// The directory does not exist, and Total Commander will not try to open it.
        /// </summary>
        public static readonly FindData NotOpen = null;

        /// <summary>
        /// The directory exists, but it's empty (Totalcmd can open it, e.g. to copy files to it).
        /// </summary>
        public static readonly FindData NoMoreFiles = new FindData();

        
        /// <summary>
        /// File attributes. Use at least the <see cref="System.IO.FileAttributes.Directory"/> flag to distinguish 
        /// between files and directories. Links should be returned as files.
        /// </summary>
        public FileAttributes Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// Local file name relative to the directory (without the path).
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// DOS-style file name (optional), set null if unused.
        /// </summary>
        public string AlternateFileName
        {
            get;
            set;
        }

        /// <summary>
        /// The size, in bytes, of the file.
        /// </summary>
        public long FileSize
        {
            get;
            set;
        }

        /// <summary>
        /// Currently unused. If available, set to the time when the file was created.
        /// </summary>
        public DateTime? CreationTime
        {
            get;
            set;
        }

        /// <summary>
        /// Currently unused. If available, set to the time when the file was last accessed.
        /// </summary>
        public DateTime? LastAccessTime
        {
            get;
            set;
        }

        /// <summary>
        /// Time stamp shown in the Total Commander file list, and copied with files.
        /// Use null for files which don't have a time.
        /// </summary>
        public DateTime? LastWriteTime
        {
            get;
            set;
        }

        /// <summary>
        /// On Unix systems, you can | (or) the <see cref="FindData.Attributes"/> field with 0x80000000 and 
        /// set the <see cref="FindData.Reserved0"/> parameter to the Unix file mode (permissions).
        /// These will then be shown in Totalcmd can can be changed through Files - Change attributes.
        /// </summary>
        public int Reserved0
        {
            get;
            set;
        }

        /// <summary>
        /// Unused, must be set to 0. Reserved for future plugin enhancements.
        /// </summary>
        public int Reserved1
        {
            get;
            set;
        }

        
        /// <summary>
        /// Initializes a new instance of the <see cref="FindData"/> class.
        /// </summary>
        public FindData()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindData"/> class.
        /// </summary>
        /// <param name="fileName">Local file name relative to the directory (without the path).</param>
        public FindData(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindData"/> class.
        /// </summary>
        /// <param name="fileName">Local file name relative to the directory (without the path).</param>
        /// <param name="fileSize">The size, in bytes, of the file.</param>
        public FindData(string fileName, long fileSize)
            : this(fileName)
        {
            FileSize = fileSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindData"/> class.
        /// </summary>
        /// <param name="fileName">Local file name relative to the directory (without the path).</param>
        /// <param name="fileSize">The size, in bytes, of the file.</param>
        /// <param name="lastWriteTime">Time stamp shown in the Total Commander file list, and copied with files.</param>
        public FindData(string fileName, long fileSize, DateTime lastWriteTime)
            : this(fileName, fileSize)
        {
            LastWriteTime = lastWriteTime;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FindData"/> class.
        /// </summary>
        /// <param name="fileName">Local file name relative to the directory (without the path).</param>
        /// <param name="attributes">File attributes.</param>
        public FindData(string fileName, FileAttributes attributes)
            : this(fileName)
        {
            Attributes = attributes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindData"/> class.
        /// </summary>
        /// <param name="fileName">Local file name relative to the directory (without the path).</param>
        /// <param name="attributes">File attributes.</param>
        /// <param name="lastWriteTime">Time stamp shown in the Total Commander file list, and copied with files.</param>
        public FindData(string fileName, FileAttributes attributes, DateTime lastWriteTime)
            : this(fileName, attributes)
        {
            LastWriteTime = lastWriteTime;
        }


        internal void CopyTo(IntPtr pFindData)
        {
            if (pFindData != IntPtr.Zero)
            {
                var findData = new FsFindData()
                {
                    FileName = FileName,
                    AlternateFileName = AlternateFileName,
                    FileAttributes = (int)Attributes,
                    FileSizeHigh = LongUtil.High(FileSize),
                    FileSizeLow = LongUtil.Low(FileSize),
                    CreationTime = DateTimeUtil.ToFileTime(CreationTime),
                    LastAccessTime = DateTimeUtil.ToFileTime(LastAccessTime),
                    LastWriteTime = DateTimeUtil.ToFileTime(LastWriteTime),
                    Reserved0 = Reserved0,
                    Reserved1 = Reserved1,
                };
                Marshal.StructureToPtr(findData, pFindData, false);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        struct FsFindData
        {
            public int FileAttributes;

            public FileTime CreationTime;
            public FileTime LastAccessTime;
            public FileTime LastWriteTime;

            public int FileSizeHigh;
            public int FileSizeLow;

            public int Reserved0;
            public int Reserved1;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.MAX_PATH)]
            public string FileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string AlternateFileName;
        }
    }
}
