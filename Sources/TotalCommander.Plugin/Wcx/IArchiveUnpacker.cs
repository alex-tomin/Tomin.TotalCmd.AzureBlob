using System;
using System.Collections.Generic;

namespace TotalCommander.Plugin.Wcx
{
    public interface IArchiveUnpacker : IEnumerator<ArchiveHeader>
    {
        void SetChangeVolume(ChangeVolume changeVolume);

        void SetProgress(Progress progress);

        
        void ExtractCurrentTo(string destfile);

        void TestCurrent(string destfile);
    }
}
