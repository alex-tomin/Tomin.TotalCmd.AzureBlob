using System;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// <see cref="Log"/> is a class, which the plugin can use to show the FTP connections toolbar, and to pass log messages to it.
    /// Totalcmd can show these messages in the log window (ftp toolbar) and write them to a log file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is received through the <see cref="ITotalCommanderWfxPlugin.Init"/> function when the plugin is loaded.
    /// </para>
    /// <para>
    /// Total Commander supports logging to files. 
    /// While one log file will store all messages, the other will only store important errors, connects, disconnects and 
    /// complete operations/transfers, but not messages by <see cref="Log.Details"/> method.
    /// </para>
    /// <para>
    /// Do NOT call <see cref="Log.Connect"/> if your plugin does not require connect/disconnect!
    /// If you call it, the function <see cref="ITotalCommanderWfxPlugin.Disconnect"/> will be called when the user presses the Disconnect button.
    /// </para>
    /// <para>
    /// Examples:<br />
    /// - FTP requires connect/disconnect, so call LogProc with MSGTYPE_CONNECT when a connection is established.<br />
    /// - Access to local file systems (e.g. Linux EXT2) does not require connect/disconnect
    /// </para>
    /// </remarks>
	public class Log
	{
        private readonly int pluginNumber;

        private readonly Log.Callback log;

		internal Log(int pluginNumber, Log.Callback log)
		{
			if (log == null) throw new ArgumentNullException("log");

			this.pluginNumber = pluginNumber;
			this.log = log;
		}

        /// <summary>
        /// Connect to a file system requiring disconnect.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// The string MUST have a specific format: "CONNECT" followed by a single whitespace, then the root of the file system 
        /// which was connected, without trailing backslash. Example: CONNECT \Filesystem.
        /// </param>
		public void Connect(string message)
		{
			LogMessage(MessageType.Connect, message);
		}
		
        /// <summary>
        /// Disconnected successfully.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// </param>
        public void Disconnect(string message)
		{
			LogMessage(MessageType.Disconnect, message);
		}

        /// <summary>
        /// Not so important messages like directory changing.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// </param>
        public void Details(string message)
		{
			LogMessage(MessageType.Details, message);
		}

        /// <summary>
        /// A file transfer was completed successfully.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// </param>
        public void TransferComplete(string message)
		{
			LogMessage(MessageType.TransferComplete, message);
		}

        /// <summary>
        /// Unused.
        /// </summary>
        /// <param name="message">
        /// String which should be logged. The string should contain both the source and target names, separated by an arrow " -> ",
        /// e.g. Download complete: \Filesystem\dir1\file1.txt -> c:\localdir\file1.txt
        /// </param>
        public void ConnectComplete(string message)
		{
			LogMessage(MessageType.ConnectComplete, message);
		}

        /// <summary>
        /// An important error has occured.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// </param>
        public void ImportantError(string message)
		{
			LogMessage(MessageType.ImportantError, message);
		}

        /// <summary>
        /// An operation other than a file transfer has completed.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// </param>
        public void OperationComplete(string message)
		{
			LogMessage(MessageType.OperationComplete, message);
		}


		private void LogMessage(MessageType messageType, string message)
		{
			log(pluginNumber, (int)messageType, message);
		}


        enum MessageType
        {
            Connect = 1,
            Disconnect,
            Details,
            TransferComplete,
            ConnectComplete,
            ImportantError,
            OperationComplete,
        }

        internal delegate void Callback(int pluginNumber, int messageType, string logString);
    }
}
