using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;
using System;
using System.Collections;
using System.IO;
using TotalCommander.Plugin.Wfx;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TotalCommander.Plugin;
using Microsoft.WindowsAzure.Storage.Auth;
using Tomin.TotalCmd.AzureBlob.Configuration;
using Tomin.TotalCmd.AzureBlob.Forms;
using System.Windows.Forms;
using Tomin.TotalCmd.AzureBlob.Properties;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using Tomin.TotalCmd.AzureBlob.Model;
using System.Text;


namespace Tomin.TotalCmd.AzureBlob
{
	public class AzureBlobWfxPlugin : TotalCommanderWfxPlugin
	{
		private static Dictionary<string, CloudBlobClient> blobClients = new Dictionary<string, CloudBlobClient>();
		private static Dictionary<string, DateTime> directoryLastWriteTimeCache = new Dictionary<string, DateTime>();

		//TODO: multithreaded support
		private bool deletionStarted = false;

		public AzureBlobWfxPlugin()
		{
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		}

		public override string PluginName { get { return "AzureBlob"; } }

		public override FindData FindFirst(string path, out IEnumerator enumerator)
		{
			//Debugger.Launch();

			if (deletionStarted)
			{
				enumerator = Enumerable.Empty<FindData>().GetEnumerator();
				return FindNext(enumerator);
			}

			var currentNode = Root.Instance.GetItemByPath(path);
#warning rebind doesn't work
//			currentNode.LoadChildren();
			enumerator = currentNode.Children.Select(x => x.ToFindData()).GetEnumerator();

			return FindNext(enumerator);
		}

		public override FindData FindNext(IEnumerator enumerator)
		{
			if (enumerator == null || !enumerator.MoveNext())
				return FindData.NoMoreFiles;

			if (enumerator.Current is FindData)
				return (FindData)enumerator.Current;

			throw new InvalidOperationException("Enumerator Type is not defined");
		}

		public override ExecuteResult ExecuteOpen(TotalCommanderWindow window, ref string remoteName)
		{
			var item = Root.Instance.GetItemByPath(remoteName);
			return item.ExecuteOpen(window, ref remoteName);
		}

		public override bool DirectoryCreate(string path)
		{
			int lastSlashIndex = path.LastIndexOf('\\');
			string existingFolderPath = path.Substring(0, lastSlashIndex);
			string newFolderName = path.Substring(lastSlashIndex + 1);

			var directory = Root.Instance.GetItemByPath(existingFolderPath);
			return directory.CreateDirectory(newFolderName);
		}

		public override FileOperationResult FileGet(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri)
		{
			var blobItem = Root.Instance.GetItemByPath(remoteName);
			return blobItem.DownloadFile(remoteName, ref  localName, copyFlags, ri);
		}

		public override FileOperationResult FilePut(string localName, ref string remoteName, CopyFlags copyFlags)
		{
			int lastSlashIndex = remoteName.LastIndexOf('\\');
			string existingFolderPath = remoteName.Substring(0, lastSlashIndex);
			string newFolderName = remoteName.Substring(lastSlashIndex + 1);

			var directory = Root.Instance.GetItemByPath(existingFolderPath);
			return directory.UploadFile(localName, newFolderName, copyFlags);
		}

		public override void StatusInfo(string remoteName, StatusOrigin origin, StatusOperation operation)
		{
			if (operation == StatusOperation.Delete && origin == StatusOrigin.Start)
			{
				deletionStarted = true;
			}
		}

		public override bool FileRemove(string remoteName)
		{
			try
			{
				deletionStarted = false;
				var item = Root.Instance.GetItemByPath(remoteName);
				item.Delete();
				return true;
			}
			catch (Exception ex)
			{
				OnError(ex);
				return false;
			}
		}

		public override bool DirectoryRemove(string remoteName)
		{
			return FileRemove(remoteName);
		}


		//TODO.
		public override FileOperationResult FileCopy(string source, string target, bool overwrite, bool move, RemoteInfo ri)
		{
			//return base.FileCopy(source, target, overwrite, move, ri);
			throw new NotImplementedException("Copy to this target Copy/Move/Rename are not implemented yet.");
		}



		public override void OnError(Exception error)
		{
			StringBuilder uiMessage = new StringBuilder("Error Occured: ")
				.Append(error.Message);
			if (error.InnerException != null)
				uiMessage.AppendFormat("\nInner Exception: {0}", error.InnerException.Message);
			Request.MessageBox(uiMessage.ToString());
			Log.ImportantError(error.ToString());
		}

		public override CustomIconResult GetCustomIcon(ref string remoteName, CustomIconFlags extractIconFlag, out System.Drawing.Icon icon)
		{
			return base.GetCustomIcon(ref remoteName, extractIconFlag, out icon);
		}

		public override BackgroundFlags BackgroundSupport
		{
			get
			{
				return BackgroundFlags.AskUser;
			}
		}
	}

}
