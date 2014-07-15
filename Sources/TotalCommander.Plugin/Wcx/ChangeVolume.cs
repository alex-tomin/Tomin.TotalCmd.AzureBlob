using System.Text;

namespace TotalCommander.Plugin.Wcx
{
    public class ChangeVolume
    {
        private readonly Callback callback;


        internal ChangeVolume(Callback callback)
        {
            this.callback = callback;
        }


        internal delegate int Callback(StringBuilder archiveName, int mode);
    }
}
