using System.Text;

namespace TotalCommander.Plugin.Wcx
{
    public class Password
    {
        private readonly Password.Callback callback;
        private readonly int cryptoNumber;

        /// <summary>
        /// It is set when the user has defined a master password.
        /// </summary>
        public bool MasterPasswordDefined
        {
            get;
            private set;
        }


        internal Password(Password.Callback callback, int cryptoNumber, int flags)
        {
            this.callback = callback;
            this.cryptoNumber = cryptoNumber;
            this.MasterPasswordDefined = flags == 1;
        }


        public ArchiveResult Save(string archiveName, string password)
        {
            return Crypt(CryptMode.SavePassword, archiveName, ref password);
        }

        public ArchiveResult Load(string archiveName, ref string password)
        {
            return Load(archiveName, false, ref password);
        }

        public ArchiveResult Load(string archiveName, bool noUI, ref string password)
        {
            return Crypt(noUI ? CryptMode.LoadPasswordNoUI : CryptMode.LoadPassword, archiveName, ref password);
        }

        public ArchiveResult Copy(string sourceArchiveName, string targetArchiveName)
        {
            return Crypt(CryptMode.CopyPassword, sourceArchiveName, ref targetArchiveName);
        }

        public ArchiveResult Move(string sourceArchiveName, string targetArchiveName)
        {
            return Crypt(CryptMode.MovePassword, sourceArchiveName, ref targetArchiveName);
        }

        public ArchiveResult Delete(string archiveName)
        {
            var password = string.Empty;
            return Crypt(CryptMode.DeletePassword, archiveName, ref password);
        }


        private ArchiveResult Crypt(CryptMode mode, string connectionName, ref string password)
        {
            var passwordBuilder = new StringBuilder(password);
            passwordBuilder.EnsureCapacity(Win32.MAX_PATH);

            var result = callback(
                cryptoNumber,
                (int)mode,
                !string.IsNullOrEmpty(connectionName) ? connectionName : null,
                passwordBuilder,
                Win32.MAX_PATH);

            password = passwordBuilder.ToString();

            return (ArchiveResult)result;
        }


        private enum CryptMode
        {
            SavePassword = 1,
            LoadPassword,
            LoadPasswordNoUI,
            CopyPassword,
            MovePassword,
            DeletePassword
        }

        internal delegate int Callback(int cryptoNumber, int mode, string connectionName, StringBuilder password, int maxLen);
    }
}
