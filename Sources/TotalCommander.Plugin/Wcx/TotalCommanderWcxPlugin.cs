using System;
using System.Collections.Generic;
using System.IO;

namespace TotalCommander.Plugin.Wcx
{
    public abstract class TotalCommanderWcxPlugin : ITotalCommanderWcxPlugin
    {
        private readonly IDictionary<IntPtr, IArchiveUnpacker> unpackers = new Dictionary<IntPtr, IArchiveUnpacker>();
        private readonly IDictionary<IntPtr, IArchivePacker> packers = new Dictionary<IntPtr, IArchivePacker>();
        private object packerConfiguration;


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
        /// </summary>
        public string PluginIniFile
        {
            get;
            private set;
        }

        public virtual BackgroundFlags BackgroundSupport
        {
            get { return BackgroundFlags.NotSupported; }
        }

        public virtual PackerCapabilities PackerCapabilities
        {
            get { return PackerCapabilities.None; }
        }

        public virtual Password Password
        {
            get;
            set;
        }


        public virtual bool CanHandleThisFile(string fileName)
        {
            return false;
        }


        public abstract IArchiveUnpacker GetUnpacker(string archiveName, OpenArchiveMode mode);

        public virtual IArchivePacker GetPacker(string archiveName, object configuration, MemoryPackMode memoryPackMode)
        {
            return null;
        }

        public virtual object ConfigurePacker(TotalCommanderWindow window)
        {
            return null;
        }


        public virtual void UnhandledException(Exception ex)
        {

        }


        void ITotalCommanderWcxPlugin.SetDefaultParams(DefaultParam dp)
        {
            PluginInterfaceVersion = dp.PluginInterfaceVersion;
            PluginIniFile = dp.DefaultIniFileName;
        }

        BackgroundFlags ITotalCommanderWcxPlugin.GetBackgroundFlags()
        {
            return BackgroundSupport;
        }

        PackerCapabilities ITotalCommanderWcxPlugin.GetPackerCapabilities()
        {
            return PackerCapabilities;
        }


        void ITotalCommanderWcxPlugin.SetPassword(Password password)
        {
            Password = password;
        }


        ArchiveResult ITotalCommanderWcxPlugin.OpenArchive(string archiveName, OpenArchiveMode mode, out IntPtr archive)
        {
            archive = IntPtr.Zero;
            var result = ArchiveResult.Default;
            var unpacker = GetUnpacker(archiveName, mode);
            if (unpacker != null)
            {
                lock (unpackers)
                {
                    unpackers[archive = new IntPtr(unpackers.Count + 1)] = unpacker;
                }
                unpacker.Reset();
                result = ArchiveResult.Success;
            }
            return result;
        }

        void ITotalCommanderWcxPlugin.SetChangeVolume(IntPtr archive, ChangeVolume changeVolume)
        {
            IArchiveUnpacker unpacker;
            lock (unpackers)
            {
                unpackers.TryGetValue(archive, out unpacker);
            }
            if (unpacker != null)
            {
                unpacker.SetChangeVolume(changeVolume);
            }
        }

        void ITotalCommanderWcxPlugin.SetProgress(IntPtr archive, Progress progress)
        {
            IArchiveUnpacker unpacker;
            lock (unpackers)
            {
                unpackers.TryGetValue(archive, out unpacker);
            }
            if (unpacker != null)
            {
                unpacker.SetProgress(progress);
            }
        }

        ArchiveResult ITotalCommanderWcxPlugin.ReadHeader(IntPtr archive, out ArchiveHeader header)
        {
            header = null;
            var result = ArchiveResult.Default;
            IArchiveUnpacker unpacker;
            lock (unpackers)
            {
                unpackers.TryGetValue(archive, out unpacker);
            }
            if (unpacker != null)
            {
                if (unpacker.MoveNext())
                {
                    header = unpacker.Current;
                    result = ArchiveResult.Success;
                }
                else
                {
                    result = ArchiveResult.EndArchive;
                }
            }
            return result;
        }

        ArchiveResult ITotalCommanderWcxPlugin.ProcessFile(IntPtr archive, ArchiveProcess operation, string filepath, string filename)
        {
            if (operation == ArchiveProcess.Skip) return ArchiveResult.Success;

            var result = ArchiveResult.Default;
            IArchiveUnpacker unpacker;
            lock (unpackers)
            {
                unpackers.TryGetValue(archive, out unpacker);
            }
            if (unpacker != null)
            {
                var destfile = !string.IsNullOrEmpty(filename) ? Path.Combine(filepath, filename) : filename;
                if (operation == ArchiveProcess.Extract)
                {
                    unpacker.ExtractCurrentTo(destfile);
                }
                else if (operation == ArchiveProcess.Test)
                {
                    unpacker.TestCurrent(destfile);
                }
                result = ArchiveResult.Success;
            }
            return result;
        }

        ArchiveResult ITotalCommanderWcxPlugin.CloseArchive(IntPtr archive)
        {
            var result = ArchiveResult.Default;
            IArchiveUnpacker unpacker;
            lock (unpackers)
            {
                if (unpackers.TryGetValue(archive, out unpacker)) unpackers.Remove(archive);
            }
            if (unpacker != null)
            {
                unpacker.Dispose();
                result = ArchiveResult.Success;
            }
            return result;
        }


        void ITotalCommanderWcxPlugin.ConfigurePacker(IntPtr window, IntPtr dllInstance)
        {
            packerConfiguration = ConfigurePacker(new TotalCommanderWindow(window));
        }

        ArchiveResult ITotalCommanderWcxPlugin.PackFiles(string archiveName, string subPath, string sourcePath, string[] addList, PackMode mode)
        {
            var result = ArchiveResult.Default;
            var packer = GetPacker(archiveName, packerConfiguration, MemoryPackMode.None);
            if (packer != null)
            {
                packer.PackFiles(subPath, sourcePath, addList, mode);
                result = ArchiveResult.Success;
            }
            return result;
        }

        ArchiveResult ITotalCommanderWcxPlugin.DeleteFiles(string archiveName, string[] deleteList)
        {
            var result = ArchiveResult.Default;
            using (var packer = GetPacker(archiveName, packerConfiguration, MemoryPackMode.None))
            {
                if (packer != null)
                {
                    packer.DeleteFiles(deleteList);
                    result = ArchiveResult.Success;
                }
            }
            return result;
        }


        IntPtr ITotalCommanderWcxPlugin.StartMemoryPack(MemoryPackMode options, string fileName)
        {
            throw new NotImplementedException();
        }

        ArchiveResult ITotalCommanderWcxPlugin.PackToMemory(IntPtr hMemPack, byte[] bufIn, ref Int32 taken, byte[] bufOut, ref Int32 written, int seekBy)
        {
            var result = ArchiveResult.Default;
            IArchivePacker packer;
            lock (packers)
            {
                packers.TryGetValue(hMemPack, out packer);
            }
            if (packer != null)
            {
                result = packer.PackInMemory(bufIn, ref taken, bufOut, ref written, seekBy) ? ArchiveResult.Done : ArchiveResult.Success;
            }
            return result;
        }

        ArchiveResult ITotalCommanderWcxPlugin.DoneMemoryPack(IntPtr hMemPack)
        {
            var result = ArchiveResult.Default;
            IArchivePacker packer;
            lock (packers)
            {
                if (packers.TryGetValue(hMemPack, out packer)) packers.Remove(hMemPack);
            }
            if (packer != null)
            {
                packer.Dispose();
                result = ArchiveResult.Success;
            }
            return result;
        }
    }
}
