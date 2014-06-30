
namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Defines constants that are returned by the <see cref="ITotalCommanderWfxPlugin.FileExecute"/> method.
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.FileExecute"/>
	public enum ExecuteResult
	{
        /// <summary>
        /// A (symbolic) link or .lnk file pointing to a different directory.
        /// </summary>
        SymLink = -2,

        /// <summary>
        /// Total Commander should download the file and execute it locally.
        /// </summary>
        YourSelf,

        /// <summary>
        /// The command was executed successfully.
        /// </summary>
		OK,

        /// <summary>
        /// Execution failed.
        /// </summary>
		Error,

        /// <summary>
        /// Default value, equal <see cref="ExecuteResult.OK"/>
        /// </summary>
        Default = OK
	}
}
