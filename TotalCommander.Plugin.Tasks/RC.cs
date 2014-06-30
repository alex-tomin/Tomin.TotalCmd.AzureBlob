using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;

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
            var sdkfilepath = ToolLocationHelper.GetPathToDotNetFrameworkSdkFile(ToolName, TargetDotNetFrameworkVersion.VersionLatest);
            if (string.IsNullOrEmpty(sdkfilepath))
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows"))
                {
                    if (key != null && key.GetValue("CurrentInstallFolder") != null)
                    {
                        sdkfilepath = Path.Combine(key.GetValue("CurrentInstallFolder").ToString(), "bin");
                    }
                }
            }
            return Path.Combine(sdkfilepath ?? string.Empty, ToolName);
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
