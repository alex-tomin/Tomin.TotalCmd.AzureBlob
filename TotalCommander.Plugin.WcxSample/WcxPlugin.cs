using TotalCommander.Plugin.Wcx;

namespace TotalCommander.Plugin.WcxSample
{
    public class WcxPlugin : TotalCommanderWcxPlugin
    {
        public override IArchiveUnpacker GetUnpacker(string archiveName, OpenArchiveMode mode)
        {

            return new WcxUnpacker(archiveName, mode);
        }

        public override PackerCapabilities PackerCapabilities
        {
            get
            {
                return PackerCapabilities.CanModify | PackerCapabilities.CanCreate | PackerCapabilities.CanDelete;
            }
        }
    }
}
