using System;
using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace TotalCommander.Plugin.Tasks
{
    public class ILAsm : ToolTask
    {
        [Required]
        public ITaskItem Source
        {
            get;
            set;
        }

        public string Resource
        {
            get;
            set;
        }

        [Output, Required]
        public string Output
        {
            get;
            set;
        }

        public string OutputType
        {
            get;
            set;
        }

        public bool NoAutoInherit
        {
            get;
            set;
        }

        public bool CreatePdb
        {
            get;
            set;
        }

        public bool Optimize
        {
            get;
            set;
        }

        public string Debug
        {
            get;
            set;
        }

        public bool Fold
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        public string Include
        {
            get;
            set;
        }

        public int Subsystem
        {
            get;
            set;
        }

        public int Flags
        {
            get;
            set;
        }

        public int Alignment
        {
            get;
            set;
        }

        public int Base
        {
            get;
            set;
        }

        public int Stack
        {
            get;
            set;
        }

        public string MDV
        {
            get;
            set;
        }

        public string MSV
        {
            get;
            set;
        }

        public bool NoCorStub
        {
            get;
            set;
        }

        public bool StripReloc
        {
            get;
            set;
        }

        public string Platform
        {
            get;
            set;
        }

        protected override string ToolName
        {
            get { return "ilasm.exe"; }
        }

        protected override string GenerateFullPathToTool()
        {
            return ToolLocationHelper.GetPathToDotNetFrameworkFile(ToolName, TargetDotNetFrameworkVersion.VersionLatest);
        }

        protected override string GetWorkingDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilder();

            builder.AppendSwitch("/nologo");
            builder.AppendSwitch("/quiet");

            builder.AppendFileNameIfNotNull(Source);
            if (Resource != null) builder.AppendSwitch("/resource=\"" + Resource + "\"");

            if (Output != null)
            {
                builder.AppendSwitch("/output=\"" + Output + "\"");
            }
            if (string.Compare(OutputType, "library", true) == 0 ||
                (string.IsNullOrEmpty(OutputType) && Output != null && !".exe".Equals(Path.GetExtension(Output), StringComparison.InvariantCultureIgnoreCase)))
            {
                builder.AppendSwitch("/dll");
            }

            if (NoAutoInherit) builder.AppendSwitch("/noautoinherit");
            if (CreatePdb) builder.AppendSwitch("/pdb");
            if (Optimize) builder.AppendSwitch("/optimize");
            if (Fold) builder.AppendSwitch("/fold");
            if (NoCorStub) builder.AppendSwitch("/nocorstub");
            if (StripReloc) builder.AppendSwitch("/stripreloc");

            if (!string.IsNullOrEmpty(Debug))
            {
                try
                {
                    var debug = Convert.ToBoolean(Debug);
                    if (debug) builder.AppendSwitch("/debug");
                }
                catch
                {
                    builder.AppendSwitch("/debug=" + Debug.ToLower());
                }
            }
            if (Key != null) builder.AppendSwitch("/key=" + Key);
            if (Include != null) builder.AppendSwitch("/include=" + Include);
            if (MDV != null) builder.AppendSwitch("/mdv=" + MDV);
            if (MSV != null) builder.AppendSwitch("/msv=" + MSV);
            if (string.Compare(Platform, "x64") == 0) builder.AppendSwitch("/x64");
            if (string.Compare(Platform, "itanium") == 0) builder.AppendSwitch("/itanium");

            if (Subsystem != 0) builder.AppendSwitch("/subsystem=" + Subsystem);
            if (Flags != 0) builder.AppendSwitch("/flags=" + Flags);
            if (Alignment != 0) builder.AppendSwitch("/alignment=" + Alignment);
            if (Base != 0) builder.AppendSwitch("/base=" + Base);
            if (Stack != 0) builder.AppendSwitch("/stack=" + Stack);

            Log.LogMessage(MessageImportance.High, "Assembling {0}...", Source);
            return builder.ToString();
        }
    }
}
