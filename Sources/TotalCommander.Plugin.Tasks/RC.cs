using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using System;

namespace TotalCommander.Plugin.Tasks
{
	public class RC : ToolTask
	{
		[Required]
		public ITaskItem Source
		{
			get;
			set;
		}

		[Output]
		public string Output
		{
			get;
			private set;
		}


		protected override string ToolName
		{
			get { return "rc.exe"; }
		}

		protected override string GenerateFullPathToTool()
		{
			throw new NotImplementedException("Please specify ToolPath parameter in msbuild targets");
		}

		protected override string GetWorkingDirectory()
		{
			return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		protected override string GenerateCommandLineCommands()
		{
			var builder = new CommandLineBuilder();
			builder.AppendSwitch("/r");
			builder.AppendFileNameIfNotNull(Source);

			Output = Path.ChangeExtension(Source.ItemSpec, ".res");

			Log.LogMessage(MessageImportance.High, "Generate res file from {0}...", Source);
			return builder.ToString();
		}
	}
}
