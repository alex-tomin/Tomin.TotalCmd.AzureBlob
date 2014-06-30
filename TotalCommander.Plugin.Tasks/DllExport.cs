using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace TotalCommander.Plugin.Tasks
{
    public class DllExport : Task
    {
        [Required]
        public string Source
        {
            get;
            set;
        }

        [Output]
        public string Output
        {
            get;
            set;
        }

        public string Encoding
        {
            get;
            set;
        }


        public override bool Execute()
        {
            var tmpfile = Path.GetTempFileName();
            try
            {
                var encoding = string.IsNullOrEmpty(Encoding) ? System.Text.Encoding.Unicode : System.Text.Encoding.GetEncoding(Encoding);
                using (var reader = new StreamReader(Source, encoding))
                using (var writer = new StreamWriter(tmpfile, false, encoding))
                {
                    var export = false;
                    var counter = 0;
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if (line == null) break;

                        line = line.Trim();
                        if (line.ToLower().StartsWith(".custom instance void ") && line.ToLower().Contains("dllexportattribute"))
                        {
                            export = true;
                        }
                        if (export && line.StartsWith("IL_0000:"))
                        {
                            writer.WriteLine(string.Format(".export [{0}]", ++counter));
                            export = false;
                        }
                        writer.WriteLine(line);
                    }
                }

                File.Copy(tmpfile, string.IsNullOrEmpty(Output) ? Source : Output, true);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
            finally
            {
                if (File.Exists(tmpfile)) File.Delete(tmpfile);
            }
        }
    }
}
