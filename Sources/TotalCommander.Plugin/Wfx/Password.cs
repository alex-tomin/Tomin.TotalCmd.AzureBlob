using System.Text;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// <see cref="Password"/> is a class, which the plugin can call to store passwords in the secure password store, 
    /// read them back, or copy them to a new connection.
    /// </summary>
    public class Password
    {
        private readonly Password.Callback callback;
        private readonly int pluginNumber;
        private readonly int cryptoNumber;

        /// <summary>
        /// It is set when the user has defined a master password.
        /// </summary>
        public bool MasterPasswordDefined
        {
            get;
            private set;
        }


        internal Password(Password.Callback callback, int pluginNumber, int cryptoNumber, int flags)
        {
            this.callback = callback;
            this.pluginNumber = pluginNumber;
            this.cryptoNumber = cryptoNumber;
            this.MasterPasswordDefined = flags == 1;
        }


        /// <summary>
        /// Save password to password store.
	    /// </summary>
        /// <param name="connection">Name of the connection for this operation.</param>
        /// <param name="password">The password to be stored.</param>
        /// <returns>
        /// Total Commander returns one of these values:<br />
        /// <list type="table">
        /// <item>
        /// <term><see cref="FileOperationResult.OK"/></term>
        /// <description>Success.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotSupported"/></term>
        /// <description>Encrypt/Decrypt failed.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.WriteError"/></term>
        /// <description>Could not write password to password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ReadError"/></term>
        /// <description>Password not found in password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotFound"/></term>
        /// <description>No master password entered yet.</description>
        /// </item>
        /// </list>
        /// </returns>
        public FileOperationResult Save(string connection, string password)
        {
            return Crypt(CryptMode.SavePassword, connection, ref password);
        }

        /// <summary>
        /// Load password from password store.
	    /// </summary>
        /// <param name="connection">Name of the connection for this operation.</param>
        /// <param name="password">The password to be retrieved.</param>
        /// <returns>
        /// Total Commander returns one of these values:<br />
        /// <list type="table">
        /// <item>
        /// <term><see cref="FileOperationResult.OK"/></term>
        /// <description>Success.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotSupported"/></term>
        /// <description>Encrypt/Decrypt failed.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.WriteError"/></term>
        /// <description>Could not write password to password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ReadError"/></term>
        /// <description>Password not found in password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotFound"/></term>
        /// <description>No master password entered yet.</description>
        /// </item>
        /// </list>
        /// </returns>
        public FileOperationResult Load(string connection, ref string password)
        {
            return Load(connection, false, ref password);
        }

        /// <summary>
        /// Load password from password store.
        /// </summary>
        /// <param name="connection">Name of the connection for this operation.</param>
        /// <param name="noUI">If <strong>true</strong> load password only if master password has already been entered.</param>
        /// <param name="password">The password to be retrieved.</param>
        /// <returns>
        /// Total Commander returns one of these values:<br />
        /// <list type="table">
        /// <item>
        /// <term><see cref="FileOperationResult.OK"/></term>
        /// <description>Success.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotSupported"/></term>
        /// <description>Encrypt/Decrypt failed.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.WriteError"/></term>
        /// <description>Could not write password to password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ReadError"/></term>
        /// <description>Password not found in password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotFound"/></term>
        /// <description>No master password entered yet.</description>
        /// </item>
        /// </list>
        /// </returns>
        public FileOperationResult Load(string connection, bool noUI, ref string password)
        {
            return Crypt(noUI ? CryptMode.LoadPasswordNoUI : CryptMode.LoadPassword, connection, ref password);
        }

        /// <summary>
        /// Copy password to new connection.
        /// </summary>
        /// <param name="sourceConnection">Name of the source connection.</param>
        /// <param name="targetConnection">Name of the target connection.</param>
        /// <returns>
        /// Total Commander returns one of these values:<br />
        /// <list type="table">
        /// <item>
        /// <term><see cref="FileOperationResult.OK"/></term>
        /// <description>Success.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotSupported"/></term>
        /// <description>Encrypt/Decrypt failed.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.WriteError"/></term>
        /// <description>Could not write password to password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ReadError"/></term>
        /// <description>Password not found in password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotFound"/></term>
        /// <description>No master password entered yet.</description>
        /// </item>
        /// </list>
        /// </returns>
        public FileOperationResult Copy(string sourceConnection, string targetConnection)
        {
            return Crypt(CryptMode.CopyPassword, sourceConnection, ref targetConnection);
        }

        /// <summary>
        /// Copy password to new connection and delete the source password.
        /// </summary>
        /// <param name="sourceConnection">Name of the source connection.</param>
        /// <param name="targetConnection">Name of the target connection.</param>
        /// <returns>
        /// Total Commander returns one of these values:<br />
        /// <list type="table">
        /// <item>
        /// <term><see cref="FileOperationResult.OK"/></term>
        /// <description>Success.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotSupported"/></term>
        /// <description>Encrypt/Decrypt failed.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.WriteError"/></term>
        /// <description>Could not write password to password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ReadError"/></term>
        /// <description>Password not found in password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotFound"/></term>
        /// <description>No master password entered yet.</description>
        /// </item>
        /// </list>
        /// </returns>
        public FileOperationResult Move(string sourceConnection, string targetConnection)
        {
            return Crypt(CryptMode.MovePassword, sourceConnection, ref targetConnection);
        }

        /// <summary>
        /// Delete the password of the given connection.
        /// </summary>
        /// <param name="connection">Name of the connection for this operation.</param>
        /// <returns>
        /// Total Commander returns one of these values:<br />
        /// <list type="table">
        /// <item>
        /// <term><see cref="FileOperationResult.OK"/></term>
        /// <description>Success.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotSupported"/></term>
        /// <description>Encrypt/Decrypt failed.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.WriteError"/></term>
        /// <description>Could not write password to password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.ReadError"/></term>
        /// <description>Password not found in password store.</description>
        /// </item>
        /// <item>
        /// <term><see cref="FileOperationResult.NotFound"/></term>
        /// <description>No master password entered yet.</description>
        /// </item>
        /// </list>
        /// </returns>
        public FileOperationResult Delete(string connection)
        {
            var password = string.Empty;
            return Crypt(CryptMode.DeletePassword, connection, ref password);
        }


        private FileOperationResult Crypt(CryptMode mode, string connectionName, ref string password)
        {
            var passwordBuilder = new StringBuilder(password);
            passwordBuilder.EnsureCapacity(Win32.MAX_PATH);

            var result = callback(
                pluginNumber,
                cryptoNumber,
                (int)mode,
                !string.IsNullOrEmpty(connectionName) ? connectionName : null,
                passwordBuilder,
                Win32.MAX_PATH);

            password = passwordBuilder.ToString();

            return (FileOperationResult)result;
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

        internal delegate int Callback(int pluginNumber, int cryptoNumber, int mode, string connectionName, StringBuilder password, int maxLen);
    }
}
