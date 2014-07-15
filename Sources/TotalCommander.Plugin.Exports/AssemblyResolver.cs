using System;
using System.IO;
using System.Reflection;

namespace TotalCommander.Plugin.Exports
{
    static class AssemblyResolver
    {
        private static bool initialized;


        public static void Init()
        {
            if (!initialized)
            {
                initialized = true;
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            }
        }


        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var file = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                new AssemblyName(args.Name).Name + ".dll"
            );
            return Assembly.LoadFrom(file);
        }
    }
}
