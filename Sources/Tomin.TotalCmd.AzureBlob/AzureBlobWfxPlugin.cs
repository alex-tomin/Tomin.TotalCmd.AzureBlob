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
using Tomin.TotalCmd.AzureBlob.Misc;
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
		private const string FakeFileName = "11FakeEmptyFile11";

		private static Dictionary<string, CloudBlobClient> blobClients = new Dictionary<string, CloudBlobClient>();
		private static Dictionary<string, DateTime> directoryLastWriteTimeCache = new Dictionary<string, DateTime>();

		public AzureBlobWfxPlugin()
		{
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		}

		public override string PluginName { get { return "AzureBlob"; } }

		public override FindData FindFirst(string path, out IEnumerator enumerator)
		{
			//Debugger.Launch();

			var currentNode = Root.Instance.GetItemByPath(path);
#warning rebind doesn't work
			//currentNode.LoadChildren();
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
			var blobPath = AzurePath.FromPath(remoteName);
			var container = blobClients[blobPath.StorageDisplayName].GetContainerReference(blobPath.ContainerName);
			var blob = container.GetBlobReferenceFromServer(blobPath.Path);
			using (var fileStream = File.OpenWrite(localName))
			{
				blob.DownloadToStream(fileStream);
			}


			return FileOperationResult.OK;
		}

		public override FileOperationResult FilePut(string localName, ref string remoteName, CopyFlags copyFlags)
		{
			var blobPath = AzurePath.FromPath(remoteName);

			if (blobPath.IsAccountOnly || blobPath.IsContainerOnly)
			{
				Request.MessageBox("You cannot create files in this level. Select a subfolder.");
				return FileOperationResult.UserAbort;
			}

			var container = blobClients[blobPath.StorageDisplayName].GetContainerReference(blobPath.ContainerName);
			var blob = container.GetBlockBlobReference(blobPath.Path);
			blob.UploadFromFile(localName, FileMode.Open);

			return FileOperationResult.OK;
		}

		//TODO.
		public override FileOperationResult FileCopy(string source, string target, bool overwrite, bool move, RemoteInfo ri)
		{
			//return base.FileCopy(source, target, overwrite, move, ri);
			throw new NotImplementedException("Copy to this target Copy/Move/Rename are not implemented yet.");
		}

		public override bool DirectoryRemove(string remoteName)
		{
			try
			{
				var blobPath = AzurePath.FromPath(remoteName);

				//if (blobPath.IsAccountOnly)
				//{
				//	Settings.Default.BlobConfigs.Remove(new BlobConfig { StorageDisplayName = blobPath.StorageDisplayName });
				//	Settings.Default.Save();
				//	blobClients.Remove(blobPath.StorageDisplayName);
				//	return true;
				//}

				var container = blobClients[blobPath.StorageDisplayName].GetContainerReference(blobPath.ContainerName);

				//no folders - only container
				if (blobPath.IsContainerOnly)
				{
					container.Delete();
				}
				else
				{
					var blobs = blobClients[blobPath.StorageDisplayName].ListBlobs(remoteName.Replace('\\', '/').Trim('/'), true);
					foreach (var blob in blobs)
					{
						((ICloudBlob)blob).Delete();
					}
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		public override bool FileRemove(string remoteName)
		{
			try
			{
				var blobPath = AzurePath.FromPath(remoteName);
				var container = blobClients[blobPath.StorageDisplayName].GetContainerReference(blobPath.ContainerName);
				var blob = container.GetBlobReferenceFromServer(blobPath.Path);
				blob.Delete();
				return true;
			}
			catch (Exception ex)
			{
				OnError(ex);
				return false;
			}
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

		public override void StatusInfo(string remoteName, StatusOrigin origin, StatusOperation operation)
		{
			
		}

		public override CustomIconResult GetCustomIcon(ref string remoteName, CustomIconFlags extractIconFlag, out System.Drawing.Icon icon)
		{
			return base.GetCustomIcon(ref remoteName, extractIconFlag, out icon);
		}



		//TODO: calculate folder times;

		private void CalculateSubfoldersLastWriteTime(AzurePath blobPath)
		{
			List<IListBlobItem> blobList = new List<IListBlobItem>();

			if (blobPath.IsAccountOnly)
			{
				var containers = blobClients[blobPath.StorageDisplayName].ListContainers();
				foreach (var container in containers)
				{
					blobList.AddRange(container.ListBlobs(useFlatBlobListing: true));
				}
			}

			else if (blobPath.IsContainerOnly)
			{
				var container = blobClients[blobPath.StorageDisplayName].GetContainerReference(blobPath.ContainerName);
				blobList.AddRange(container.ListBlobs(useFlatBlobListing: true));
			}
			else
			{
				var container = blobClients[blobPath.StorageDisplayName].GetContainerReference(blobPath.ContainerName);
				blobList.AddRange(container.GetDirectoryReference(blobPath.Path).ListBlobs(useFlatBlobListing: true));
			}

			foreach (var blob in blobList.OfType<ICloudBlob>())
			{
				foreach (var folder in GetFolders(blob.Uri.AbsolutePath))
				{
					if (blob.Properties.LastModified == null)
						continue;

					DateTime lastModified = blob.Properties.LastModified.Value.ToLocalTime().DateTime;
					if (!directoryLastWriteTimeCache.ContainsKey(folder))
					{
						directoryLastWriteTimeCache[folder] = lastModified;
					}
					else
					{
						directoryLastWriteTimeCache[folder] = directoryLastWriteTimeCache[folder] > lastModified ? directoryLastWriteTimeCache[folder] : lastModified;
					}
				}
			}
		}

		private IEnumerable<string> GetFolders(string path)
		{
			if (string.IsNullOrEmpty(path))
				yield break;
			for (int index = 1; ; index++)
			{
				index = path.IndexOf('/', index);
				if (index == -1)
					break;
				yield return path.Substring(0, index + 1);
			}
		}
	}

}
