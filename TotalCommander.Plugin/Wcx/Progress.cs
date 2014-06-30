using System.Text;

namespace TotalCommander.Plugin.Wcx
{
    public class Progress
    {
        private readonly Callback callback;


        internal Progress(Callback callback)
        {
            this.callback = callback;
        }


        public bool SetProgress(string filename, int unpackedBytes)
        {
            return callback(filename, unpackedBytes) != 0;
        }

        public bool SetFirstProgress(string filename, int percent)
        {
            return callback(filename, -percent) != 0;
        }

        public bool SetSecondProgress(string filename, int percent)
        {
            return callback(filename, -1000 - percent) != 0;
        }


        internal delegate int Callback(string filename, int size);
    }
}
