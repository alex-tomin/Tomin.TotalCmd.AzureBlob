using System;
using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;

namespace TotalCommander.Plugin.Tasks
{
    public class ILDasm : ToolTask
    {
        [Required]
        public ITaskItem Source
        {
            get;
            set;
        }

        [Output, Required]
        public string OutputIL
        {
            get;
            set;
        }

        [Output]
        public string OutputRes
        {
            get;
            private set;
        }

        public string Encoding
        {
            get;
            set;
        }

        public string Item
        {
            get;
            set;
        }


        protected override string ToolName
        {
            get { return "ildasm.exe"; }
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
			return sdkfilepath;
        }

        protected override string GetWorkingDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilder();

            builder.AppendFileNameIfNotNull(Source);
            if (Source != null)
            {
                OutputRes = Path.ChangeExtension(OutputIL, ".res");
            }

            builder.AppendSwitch("/nobar");
            builder.AppendSwitchIfNotNull("/output=", OutputIL);
            if (Encoding == null || Encoding.StartsWith("uni", StringComparison.InvariantCultureIgnoreCase))
            {
                builder.AppendSwitch("/unicode");
            }
            if (Encoding != null && Encoding.StartsWith("utf", StringComparison.InvariantCultureIgnoreCase))
            {
                builder.AppendSwitch("/utf8");
            }
            builder.AppendSwitchIfNotNull("/item:", Item);

            Log.LogMessage(MessageImportance.High, "Disassembling {0}...", Source);
            return builder.ToString();
        }
    }
}
