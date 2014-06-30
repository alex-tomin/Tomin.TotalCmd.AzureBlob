using System;
using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wcx
{
    public class OpenArchiveInfo
    {
        private readonly IntPtr ptr;
        private OpenArchiveData data;


        public string ArchiveName
        {
            get { return data.ArchiveName; }
        }

        public OpenArchiveMode Mode
        {
            get { return (OpenArchiveMode)data.Mode; }
        }

        public ArchiveResult Result
        {
            get
            {
                return (ArchiveResult)data.Result;
            }
            set
            {
                if (ptr != IntPtr.Zero)
                {
                    data.Result = (int)value;
                    Marshal.StructureToPtr(data, ptr, false);
                }
            }
        }


        internal OpenArchiveInfo(IntPtr ptr)
        {
            this.ptr = ptr;
            if (ptr != IntPtr.Zero)
            {
                data = (OpenArchiveData)Marshal.PtrToStructure(ptr, typeof(OpenArchiveData));
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct OpenArchiveData
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ArchiveName;

            public Int32 Mode;

            public Int32 Result;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string CommentBuffer;

            public Int32 CommentBufferSize;

            public Int32 CommentSize;

            public Int32 CommentState;
        }
    }
}
