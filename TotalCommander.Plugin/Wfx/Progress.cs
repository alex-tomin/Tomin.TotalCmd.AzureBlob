using System;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// <see cref="Progress"/> is a class, which the plugin can use to show copy progress.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is received through the <see cref="ITotalCommanderWfxPlugin.Init"/> 
    /// function when the plugin is loaded.
    /// </para>
    /// <para>
    /// You should call this function at least twice in the copy functions 
    /// <see cref="ITotalCommanderWfxPlugin.FileGet"/>, <see cref="ITotalCommanderWfxPlugin.FilePut"/> and 
    /// <see cref="ITotalCommanderWfxPlugin.FileRenameMove"/>, at the beginning and at the end.
    /// If you can't determine the progress, call it with 0% at the beginning and 100% at the end.
    /// </para>
    /// <para>
    /// New in 1.3: During the <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/>/<see cref="ITotalCommanderWfxPlugin.FindClose"/> loop, 
    /// the plugin may now call the <see cref="Progress.SetProgress"/> to make a progess dialog appear.
    /// This is useful for very slow connections. Don't call <see cref="Progress.SetProgress"/> for fast connections!
    /// The progress dialog will only be shown for normal dir changes, not for compound operations like get/put. 
    /// The calls to <see cref="Progress.SetProgress"/> will also be ignored during the first 5 seconds, 
    /// so the user isn't bothered with a progress dialog on every dir change.
    /// </para>
    /// </remarks>
	public class Progress
	{
        private readonly int pluginNumber;

        private readonly Progress.Callback progress;

		internal Progress(int pluginNumber, Progress.Callback progress)
		{
			if (progress == null) throw new ArgumentNullException("progress");

			this.pluginNumber = pluginNumber;
			this.progress = progress;
		}

        /// <summary>
        /// Show copy progress.
        /// </summary>
        /// <param name="source">
        /// Name of the source file being copied. 
        /// Depending on the direction of the operation (Get, Put), this may be a local file name of a name in the plugin file system.
        /// </param>
        /// <param name="target">
        /// Name to which the file is copied.
        /// </param>
        /// <param name="percent">
        /// Percentage of THIS file being copied. 
        /// Total Commander automatically shows a second percent bar if possible when multiple files are copied.
        /// </param>
        /// <returns>
        /// Total Commander returns <strong>false</strong> if the user wants to abort copying, and <strong>true</strong> if the operation can continue.
        /// </returns>
		public bool SetProgress(string source, string target, int percent)
		{
            return progress(pluginNumber, source, target, percent) == 0;
		}


        internal delegate int Callback(int pluginNumber, string sourceName, string targetName, int percentDone);
	}
}
