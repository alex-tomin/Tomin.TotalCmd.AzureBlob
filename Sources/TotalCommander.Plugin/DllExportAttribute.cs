using System;

namespace TotalCommander.Plugin
{
    /// <summary>
    /// Indicates that the attributed method is exported from assembly.
    /// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class DllExportAttribute : Attribute
	{
	}
}
