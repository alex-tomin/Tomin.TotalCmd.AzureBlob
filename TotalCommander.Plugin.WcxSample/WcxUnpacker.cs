using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TotalCommander.Plugin.Wcx;

namespace TotalCommander.Plugin.WcxSample
{
    class WcxUnpacker : IArchiveUnpacker
    {
        private readonly IEnumerator<string> files;
        private readonly string archivename;
        private readonly OpenArchiveMode mode;
        private Progress progress;
        private ChangeVolume changeVol;


        public WcxUnpacker(string archivename, OpenArchiveMode mode)
        {
            this.archivename = archivename;
            this.mode = mode;
            files = Directory.GetFiles("d:\\", "*.*").ToList().GetEnumerator();
        }

        public void ExtractCurrentTo(string destfile)
        {
            for (int i = 0; i < 10; i++)
            {
                if (!progress.SetFirstProgress(files.Current, i * 10)) return;
                if (!progress.SetSecondProgress(files.Current, i * 5)) return;
                System.Threading.Thread.Sleep(500);
            }
        }

        public void TestCurrent(string destfile)
        {

        }

        public ArchiveHeader Current
        {
            get;
            private set;
        }

        public void Dispose()
        {
            progress.SetProgress("ddd", -100);
            progress.SetProgress("ddd", -1100);
            files.Dispose();
        }

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            var result = files.MoveNext();
            if (result)
            {
                var fi = new FileInfo(files.Current);
                Current = new ArchiveHeader
                {
                    ArchiveName = archivename,
                    FileAttributes = fi.Attributes,
                    FileName = fi.Name,
                    FileTime = fi.LastWriteTime,
                    UnpackedSize = fi.Length,
                    PackedSize = fi.Length,
                };
            }
            else
            {
                Current = null;
            }
            return result;
        }

        public void Reset()
        {
            files.Reset();
        }

        public void SetChangeVolume(ChangeVolume changeVolume)
        {
            changeVol = changeVolume;
        }

        public void SetProgress(Progress progress)
        {
            this.progress = progress;
        }
    }
}
