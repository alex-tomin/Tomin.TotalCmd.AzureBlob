using System;
using System.Text;
using System.Windows.Forms;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// <see cref="Request"/> is a class, which the plugin can use to request input from the user.
    /// </summary>
    /// <remarks>
    /// This class is received through the <see cref="ITotalCommanderWfxPlugin.Init"/> function when the plugin is loaded.
    /// </remarks>
    public class Request
    {
        private readonly int pluginNumber;
        private readonly string pluginName;
        private readonly Request.Callback request;

        internal Request(int pluginNumber, string pluginName, Request.Callback request)
        {
            if (request == null) throw new ArgumentNullException("request");

            this.pluginNumber = pluginNumber;
            this.pluginName = pluginName;
            this.request = request;
        }

        #region UserName

        /// <summary>
        /// Ask for the user name, e.g. for a connection.
        /// </summary>
        /// <param name="username">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="username"/> = null to have no default text.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUserName(ref string username)
        {
            return GetRequest(RequestType.UserName, null, null, ref username);
        }

        /// <summary>
        /// Ask for the user name, e.g. for a connection.
        /// </summary>
        /// <param name="username">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="username"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUserName(ref string username, string text)
        {
            return GetRequest(RequestType.UserName, text, null, ref username);
        }

        /// <summary>
        /// Ask for the user name, e.g. for a connection.
        /// </summary>
        /// <param name="username">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="username"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <param name="caption">Custom title for the dialog box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUserName(ref string username, string text, string caption)
        {
            return GetRequest(RequestType.UserName, text, caption, ref username);
        }

        #endregion UserName


        #region Password

        /// <summary>
        /// Ask for a password, e.g. for a connection (shows ***).
        /// </summary>
        /// <param name="password">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="password"/> = null to have no default text.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetPassword(ref string password)
        {
            return GetRequest(RequestType.Password, null, null, ref password);
        }

        /// <summary>
        /// Ask for a password, e.g. for a connection (shows ***).
        /// </summary>
        /// <param name="password">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="password"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetPassword(ref string password, string text)
        {
            return GetRequest(RequestType.Password, text, null, ref password);
        }

        /// <summary>
        /// Ask for a password, e.g. for a connection (shows ***).
        /// </summary>
        /// <param name="password">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="password"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <param name="caption">Custom title for the dialog box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetPassword(ref string password, string text, string caption)
        {
            return GetRequest(RequestType.Password, text, caption, ref password);
        }

        #endregion Password


        #region Account

        /// <summary>
        /// Ask for an account (needed for some FTP servers).
        /// </summary>
        /// <param name="account">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="account"/> = null to have no default text.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetAccount(ref string account)
        {
            return GetRequest(RequestType.Account, null, null, ref account);
        }

        /// <summary>
        /// Ask for an account (needed for some FTP servers).
        /// </summary>
        /// <param name="account">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="account"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetAccount(ref string account, string text)
        {
            return GetRequest(RequestType.Account, text, null, ref account);
        }

        /// <summary>
        /// Ask for an account (needed for some FTP servers).
        /// </summary>
        /// <param name="account">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="account"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <param name="caption">Custom title for the dialog box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetAccount(ref string account, string text, string caption)
        {
            return GetRequest(RequestType.Account, text, caption, ref account);
        }

        #endregion Account


        #region UserNameFirewall

        /// <summary>
        /// User name for a firewall.
        /// </summary>
        /// <param name="userNameFirewall">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="userNameFirewall"/> = null to have no default text.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUserNameFirewall(ref string userNameFirewall)
        {
            return GetRequest(RequestType.UserNameFirewall, null, null, ref userNameFirewall);
        }

        /// <summary>
        /// User name for a firewall.
        /// </summary>
        /// <param name="userNameFirewall">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="userNameFirewall"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUserNameFirewall(ref string userNameFirewall, string text)
        {
            return GetRequest(RequestType.UserNameFirewall, text, null, ref userNameFirewall);
        }

        /// <summary>
        /// User name for a firewall.
        /// </summary>
        /// <param name="userNameFirewall">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="userNameFirewall"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <param name="caption">Custom title for the dialog box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUserNameFirewall(ref string userNameFirewall, string text, string caption)
        {
            return GetRequest(RequestType.UserNameFirewall, text, caption, ref userNameFirewall);
        }

        #endregion UserNameFirewall


        #region PasswordFirewall

        /// <summary>
        /// Password for a firewall.
        /// </summary>
        /// <param name="passwordFirewall">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="passwordFirewall"/> = null to have no default text.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetPasswordFirewall(ref string passwordFirewall)
        {
            return GetRequest(RequestType.PasswordFirewall, null, null, ref passwordFirewall);
        }

        /// <summary>
        /// Password for a firewall.
        /// </summary>
        /// <param name="passwordFirewall">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="passwordFirewall"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetPasswordFirewall(ref string passwordFirewall, string text)
        {
            return GetRequest(RequestType.PasswordFirewall, text, null, ref passwordFirewall);
        }

        /// <summary>
        /// Password for a firewall.
        /// </summary>
        /// <param name="passwordFirewall">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="passwordFirewall"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <param name="caption">Custom title for the dialog box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetPasswordFirewall(ref string passwordFirewall, string text, string caption)
        {
            return GetRequest(RequestType.PasswordFirewall, text, caption, ref passwordFirewall);
        }

        #endregion PasswordFirewall


        #region TargetDir

        /// <summary>
        /// Asks for a local directory (with browse button).
        /// </summary>
        /// <param name="targetDir">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="targetDir"/> = null to have no default text.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetTargetDir(ref string targetDir)
        {
            return GetRequest(RequestType.TargetDir, null, null, ref targetDir);
        }

        /// <summary>
        /// Asks for a local directory (with browse button).
        /// </summary>
        /// <param name="targetDir">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="targetDir"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetTargetDir(ref string targetDir, string text)
        {
            return GetRequest(RequestType.TargetDir, text, null, ref targetDir);
        }

        /// <summary>
        /// Asks for a local directory (with browse button).
        /// </summary>
        /// <param name="targetDir">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="targetDir"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <param name="caption">Custom title for the dialog box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetTargetDir(ref string targetDir, string text, string caption)
        {
            return GetRequest(RequestType.TargetDir, text, caption, ref targetDir);
        }

        #endregion TargetDir


        #region Url

        /// <summary>
        /// Asks for an URL.
        /// </summary>
        /// <param name="url">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="url"/> = null to have no default text.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUrl(ref string url)
        {
            return GetRequest(RequestType.URL, null, null, ref url);
        }

        /// <summary>
        /// Asks for an URL.
        /// </summary>
        /// <param name="url">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="url"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUrl(ref string url, string text)
        {
            return GetRequest(RequestType.URL, text, null, ref url);
        }

        /// <summary>
        /// Asks for an URL.
        /// </summary>
        /// <param name="url">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="url"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <param name="caption">Custom title for the dialog box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetUrl(ref string url, string text, string caption)
        {
            return GetRequest(RequestType.URL, text, caption, ref url);
        }

        #endregion Url


        #region MessageBox

        /// <summary>
        /// Shows MessageBox.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool MessageBox(string text)
        {
            return GetRequest(RequestType.MsgOK, text, null);
        }

        /// <summary>
        /// Shows MessageBox.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool MessageBox(string text, string caption)
        {
            return GetRequest(RequestType.MsgOK, text, caption);
        }

        /// <summary>
        /// Shows MessageBox.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the <see cref="MessageBoxButtons"/>
        /// values that specifies which buttons to display in the message box.
        /// Only <see cref="MessageBoxButtons.OK"/>, <see cref="MessageBoxButtons.OKCancel"/> 
        /// and <see cref="MessageBoxButtons.YesNo"/> supported.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool MessageBox(string text, MessageBoxButtons buttons)
        {
            return GetRequest(ResolveRequestType(buttons), text, null);
        }

        /// <summary>
        /// Shows MessageBox.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the <see cref="MessageBoxButtons"/>
        /// values that specifies which buttons to display in the message box.
        /// Only <see cref="MessageBoxButtons.OK"/>, <see cref="MessageBoxButtons.OKCancel"/> 
        /// and <see cref="MessageBoxButtons.YesNo"/> supported.
        /// </param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool MessageBox(string text, string caption, MessageBoxButtons buttons)
        {
            return GetRequest(ResolveRequestType(buttons), text, caption);
        }

        #endregion MessageBox


        #region 

        /// <summary>
        /// Asks for a string.
        /// </summary>
        /// <param name="returned">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="returned"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetOther(ref string returned, string text)
        {
            return GetRequest(RequestType.Other, text, null, ref returned);
        }

        /// <summary>
        /// Asks for a string.
        /// </summary>
        /// <param name="returned">
        /// This string contains the default text presented to the user, and will receive the (modified) text which the user enters.
        /// Set <paramref name="returned"/> = null to have no default text.
        /// </param>
        /// <param name="text">Override the default text.</param>
        /// <param name="caption">Custom title for the dialog box.</param>
        /// <returns>Returns <strong>true</strong> if the user clicked OK or Yes, <strong>false</strong> otherwise.</returns>
        public bool GetOther(ref string returned, string text, string caption)
        {
            return GetRequest(RequestType.Other, text, caption, ref returned);
        }

        #endregion


        private bool GetRequest(RequestType requestType, string text, string caption)
        {
            string result = null;
            return GetRequest(requestType, text, caption, ref result);
        }

        private bool GetRequest(RequestType requestType, string text, string caption, ref string result)
        {
            var resultBuilder = new StringBuilder(result ?? string.Empty);
            resultBuilder.EnsureCapacity(Win32.MAX_PATH);

            var ok = request(
                pluginNumber,
                (int)requestType,
                !string.IsNullOrEmpty(caption) ? caption : pluginName,
                !string.IsNullOrEmpty(text) ? text : null,
                resultBuilder,
                Win32.MAX_PATH);

            result = resultBuilder.ToString();

            return ok;
        }

        private RequestType ResolveRequestType(MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.OKCancel: return RequestType.MsgOKCancel;
                case MessageBoxButtons.YesNo: return RequestType.MsgYesNo;
                default: return RequestType.MsgOK;
            }
        }


        enum RequestType
        {
            Other,
            UserName,
            Password,
            Account,
            UserNameFirewall,
            PasswordFirewall,
            TargetDir,
            URL,
            MsgOK,
            MsgYesNo,
            MsgOKCancel
        }

        internal delegate bool Callback(int pluginNumber, int requestType, string customTitle, string customText, StringBuilder defaultText, int maxLen);
    }
}
