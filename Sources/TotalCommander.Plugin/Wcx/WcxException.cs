using System;
using System.Collections.Generic;
using System.Text;

namespace TotalCommander.Plugin.Wcx
{
    public class WcxException : Exception
    {
        public ArchiveResult ArchiveResult
        {
            get;
            private set;
        }


        public WcxException(ArchiveResult archiveResult)
        {
            ArchiveResult = archiveResult;
        }

        public WcxException(ArchiveResult archiveResult, string message)
            : base(message)
        {
            ArchiveResult = archiveResult;
        }

        public WcxException(ArchiveResult archiveResult, string message, Exception inner)
            : base(message, inner)
        {
            ArchiveResult = archiveResult;
        }
    }
}
