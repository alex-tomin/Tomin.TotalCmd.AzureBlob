using System;
using System.IO;
using System.Reflection;
using TotalCommander.Plugin.Wcx;
using TotalCommander.Plugin.Wfx;

namespace TotalCommander.Plugin
{
    static class TotalCommanderPluginHolder
    {
        private static ITotalCommanderWfxPlugin wfx;

        private static ITotalCommanderWcxPlugin wcx;


        static TotalCommanderPluginHolder()
        {
            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                foreach (var file in Directory.GetFiles(path, "*.dll"))
                {
                    if (string.Compare(Assembly.GetExecutingAssembly().Location, file, true) == 0) continue;
                
                    var assembly = Assembly.LoadFrom(file);
                    foreach (var type in assembly.GetExportedTypes())
                    {
                        var interfaces = type.GetInterfaces();
                        if (Array.Exists(interfaces, i => i == typeof(ITotalCommanderWfxPlugin)))
                        {
                            wfx = (ITotalCommanderWfxPlugin)Activator.CreateInstance(type);
                            return;
                        }
                        if (Array.Exists(interfaces, i => i == typeof(ITotalCommanderWcxPlugin)))
                        {
                            wcx = (ITotalCommanderWcxPlugin)Activator.CreateInstance(type);
                            return;
                        }
                    }
                }
            }
            catch { }
        }


        public static ITotalCommanderWfxPlugin GetWfxPlugin()
        {
            return wfx;
        }

        public static ITotalCommanderWcxPlugin GetWcxPlugin()
        {
            return wcx;
        }
    }
}
