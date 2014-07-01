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


namespace Tomin.TotalCmd.AzureBlob
{
	public class AzureBlobWfxPlugin : TotalCommanderWfxPlugin
	{

		private const string AddNewStorageText = "<Add New Storage...>.";
		private const string DoNotUseDelete = "!WARN: do not use Del/F8 in this folder.";
		private const string FakeFileName = "11FakeEmptyFile11";

		private static Dictionary<string, CloudBlobClient> blobClients = new Dictionary<string, CloudBlobClient>();

		public AzureBlobWfxPlugin()
		{
		}

		public override string PluginName
		{
			get { return "AzureBlob"; }
		}

		public override FindData FindFirst(string path, out IEnumerator enumerator)
		{
			//Debugger.Launch();
			//root folder.
			if (path == "\\")
			{
				enumerator = GetStorageAccounts().GetEnumerator();
				return FindNext(enumerator);
			}

			var blobPath = AzurePath.FromPath(path);

			if (blobPath.IsAccountOnly)
			{
				enumerator = blobClients[blobPath.StorageDisplayName].ListContainers().GetEnumerator();
				return FindNext(enumerator);
			}

			var container = blobClients[blobPath.StorageDisplayName].GetContainerReference(blobPath.ContainerName);

			if (blobPath.IsContainerOnly)
			{
				enumerator = container.ListBlobs().GetEnumerator();
				return FindNext(enumerator);
			}
			else
			{
				enumerator = container.GetDirectoryReference(blobPath.Path).ListBlobs().GetEnumerator();
				return FindNext(enumerator);
			}
		}

		public override FindData FindNext(IEnumerator enumerator)
		{
			if (enumerator == null || !enumerator.MoveNext())
				return FindData.NoMoreFiles;

			if (enumerator.Current is FindData)
				return (FindData)enumerator.Current;

			if (enumerator.Current is CloudBlobContainer)
				return ContainerToFindData((CloudBlobContainer)enumerator.Current);

			if (enumerator.Current is CloudBlobDirectory)
				return DirectoryToFindData((CloudBlobDirectory)enumerator.Current);

			if (enumerator.Current is ICloudBlob)
			{
				FindData findData = BlobToFindData((ICloudBlob)enumerator.Current);
				if (findData.FileName.Contains(FakeFileName))
					return FindNext(enumerator);
				return findData;
			}

			return FindData.NoMoreFiles;
		}

		public override bool DirectoryCreate(string path)
		{
			try
			{
				var blobPath = AzurePath.FromPath(path);

				if (blobPath.IsAccountOnly)
				{
					Request.MessageBox("You canot create Account. Use " + AddNewStorageText + " below");
					return false;
				}

				if (blobPath.IsContainerOnly)
				{
					var newContainer = blobClients[blobPath.StorageDisplayName].GetContainerReference(blobPath.ContainerName);
					newContainer.CreateIfNotExists();
					return true;
				}
				
				//create virtual folder 

				var container = blobClients[blobPath.StorageDisplayName].GetContainerReference(blobPath.ContainerName);
				var blob = container.GetBlockBlobReference(String.Format("{0}/{1}", blobPath.Path, FakeFileName));
				blob.UploadText(string.Empty);

				return true;
			}
			catch(Exception ex)
			{
				Request.MessageBox("Error while creating folder. Probably you used not allowed symbols in name. Message: " + ex.Message);
				return false;
			}
		}

		public override ExecuteResult ExecuteOpen(TotalCommanderWindow window, ref string remoteName)
		{
			if (!remoteName.Contains(AddNewStorageText))
				return ExecuteResult.YourSelf;

			//adding new storage account
			var addStorageDialog = new AddStorageDialog();
			if (addStorageDialog.ShowDialog() != DialogResult.OK)
				return ExecuteResult.OK;

			var blobConfig = addStorageDialog.StorageConfigInfo;

			if (blobClients.ContainsKey(blobConfig.StorageDisplayName))
				throw new Exception("Display Name already exists. Please give another name.");

			Settings.Default.BlobConfigs.Add(blobConfig);
			ReloadBlobClientsFromConfig();
			Settings.Default.Save();

			//redirect to Root folder, so it reflects the newly added folder
			remoteName = @"\";
			return ExecuteResult.SymLink;

		}

		public override FileOperationResult FileGet(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri)
		{
			var blobPath = AzurePath.FromPath(remoteName);
			var container = blobClients[blobPath.StorageDisplayName].GetContainerReference(blobPath.ContainerName);
			var blob = container.GetBlockBlobReference(blobPath.Path);

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

		public override FileOperationResult FileCopy(string source, string target, bool overwrite, bool move, RemoteInfo ri)
		{
			return base.FileCopy(source, target, overwrite, move, ri);
		}

		public override bool DirectoryRemove(string remoteName)
		{
			try
			{
				var blobPath = AzurePath.FromPath(remoteName);

				if (blobPath.IsAccountOnly)
				{
					Settings.Default.BlobConfigs.Remove(new BlobConfig { StorageDisplayName = blobPath.StorageDisplayName });
					Settings.Default.Save();
					blobClients.Remove(blobPath.StorageDisplayName);
					return true;
				}

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
				var blob = container.GetBlockBlobReference(blobPath.Path);
				blob.Delete();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public override void OnError(Exception error)
		{
			Request.MessageBox(error.Message);
			Log.ImportantError(error.Message);
		}

		public override CustomIconResult GetCustomIcon(ref string remoteName, CustomIconFlags extractIconFlag, out System.Drawing.Icon icon)
		{
			return base.GetCustomIcon(ref remoteName, extractIconFlag, out icon);
		}

		//-- Helpers

		private IEnumerable<FindData> GetStorageAccounts()
		{
			if (!blobClients.Any())
				ReloadBlobClientsFromConfig();

			return blobClients.Keys.Select(x => new FindData(x, FileAttributes.Directory))
				.Concat(new[] { 
					new FindData(AddNewStorageText),
					new FindData(DoNotUseDelete)
				});
		}

		private static void ReloadBlobClientsFromConfig()
		{
			blobClients = Settings.Default.BlobConfigs.ToDictionary(
						s => s.StorageDisplayName,
						s => s.UseDevelopmentStorage
							? CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient()
							: new CloudStorageAccount(new StorageCredentials(s.AccountName, s.AccountKey), s.UseSsl).CreateCloudBlobClient()
					);
		}

		private FindData ContainerToFindData(CloudBlobContainer container)
		{
			var findData = new FindData(container.Name);
			findData.Attributes |= FileAttributes.Directory;
			if (container.Properties.LastModified != null)
				findData.LastWriteTime = container.Properties.LastModified.Value.ToLocalTime().DateTime;

			return findData;
		}

		private FindData DirectoryToFindData(CloudBlobDirectory blobItem)
		{
			var findData = new FindData(Uri.UnescapeDataString(blobItem.Uri.Segments.Last().TrimEnd('/')));
			findData.Attributes |= FileAttributes.Directory;
			return findData;
		}

		private FindData BlobToFindData(ICloudBlob blobItem)
		{
			var findData = new FindData(Uri.UnescapeDataString(blobItem.Uri.Segments.Last().TrimEnd('/')), blobItem.Properties.Length);
			if (blobItem.Properties.LastModified != null)
				findData.LastWriteTime = blobItem.Properties.LastModified.Value.ToLocalTime().DateTime;
			return findData;
		}
	}

}
