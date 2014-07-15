using System;
using System.Runtime.InteropServices;

namespace TotalCommander.Plugin
{
    /// <summary>
    /// <see cref="DefaultParam"/> contains information about the current plugin interface version and ini file location.
    /// </summary>
    public class DefaultParam
    {
        /// <summary>
        /// Plugin interface version.
        /// </summary>
        public Version PluginInterfaceVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Suggested location+name of the ini file where the plugin could store its data.
        /// This is a fully qualified path+file name, and will be in the same directory as the wincmd.ini.
        /// It's recommended to store the plugin data in this file or at least in this directory,
        /// because the plugin directory or the Windows directory may not be writable!
        /// </summary>
        public string DefaultIniFileName
        {
            get;
            private set;
        }


        internal DefaultParam(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                var param = (DefaultParamStruct)Marshal.PtrToStructure(ptr, typeof(DefaultParamStruct));
                PluginInterfaceVersion = new Version(param.VersionHigh, param.VersionLow / 10);
                DefaultIniFileName = param.DefaultIniName;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct DefaultParamStruct
        {
            public int Size;

            public int VersionLow;

            public int VersionHigh;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string DefaultIniName;
        };
    };
}
