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
		enum DeletionState
		{
			None = 0,
			Initiated,
			Enumerated
		}

		//private static Dictionary<string, CloudBlobClient> blobClients = new Dictionary<string, CloudBlobClient>();
		private static Dictionary<string, DateTime> directoryLastWriteTimeCache = new Dictionary<string, DateTime>();
		private readonly TimeSpan cacheDuration = TimeSpan.FromSeconds(3); //Todo: move to config

		//TODO: multithreaded support
		/// <summary>
		/// Used to improve directory deletion. See FindFirst function - inline comments.
		/// </summary>
		private DeletionState deletionState = DeletionState.None;

		public AzureBlobWfxPlugin()
		{
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		}

		public override string PluginName { get { return "AzureBlob"; } }

		public override FindData FindFirst(string path, out IEnumerator enumerator)
		{
			var currentNode = Root.Instance.GetItemByPath(path);

			//Special handling for deletion, as TotalCMD doesn't allow batch deletion. 
			//It first enumerates all items, then removes one-by-one, and only then removes empty fodlers. - It could take hours if you have big an deep storage.
			if (deletionState == DeletionState.Initiated)
			{
				if (currentNode is BlobDirectory)
				{
					//get all files in a flat way - it is faster
					((BlobDirectory)currentNode).LoadAllSubItems();
					deletionState = DeletionState.Enumerated; //TotalCMD triggers enumeration twice, we want only once. 
				}
				else
				{
					//don't enumerate items - proceed to folder deletion immediately
					enumerator = Enumerable.Empty<FindData>().GetEnumerator();
					return FindNext(enumerator);
				}
			}
			else if (deletionState == DeletionState.None)
			{
				currentNode.LoadChildren(cacheDuration);
			}

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
			bool cancelled = !Progress.SetProgress(remoteName, localName, 0);
			if (cancelled)
				return FileOperationResult.UserAbort;

			if (File.Exists(localName))
			{
				//First time? - show user a prompt.
				if (copyFlags == CopyFlags.None || copyFlags == CopyFlags.Move)
					return FileOperationResult.Exists;
			}

			var blobItem = Root.Instance.GetItemByPath(remoteName);
			var cancellationToken = new CancellationTokenSource();
			var name = localName;

			Action<int> setProgress = progress =>
			{
				cancelled = !Progress.SetProgress(remoteName, name, progress);
				if (cancelled) cancellationToken.Cancel();
			};

			try
			{
				blobItem.DownloadFile(remoteName, localName, copyFlags, cancellationToken.Token, setProgress);
			}
			catch (Exception ex)
			{
				if (!(ex.InnerException is OperationCanceledException))
					throw;

				//remove half downloaded file
				if (File.Exists(localName))
					File.Delete(localName);
				return FileOperationResult.UserAbort;
			}

			if ((copyFlags & CopyFlags.Move) == CopyFlags.Move)
			{
				blobItem.Delete(); //Remove original on Move operation
			}

			Progress.SetProgress(remoteName, localName, 100);
			return FileOperationResult.OK;
		}

		public override FileOperationResult FilePut(string localName, ref string remoteName, CopyFlags copyFlags)
		{
			bool cancelled = !Progress.SetProgress(remoteName, localName, 0);
			if (cancelled)
				return FileOperationResult.UserAbort;

			FileSystemItemBase remoteBlob = null;
			if (Root.Instance.TryGetItemByPath(remoteName, out remoteBlob))
			{
				//First time? - show user a prompt.
				if ((copyFlags & CopyFlags.Overwrite) != CopyFlags.Overwrite)
					return FileOperationResult.Exists;
			}

			int lastSlashIndex = remoteName.LastIndexOf('\\');
			string existingFolderPath = remoteName.Substring(0, lastSlashIndex);
			string newFolderName = remoteName.Substring(lastSlashIndex + 1);

			var directory = Root.Instance.GetItemByPath(existingFolderPath);
			var cancellationToken = new CancellationTokenSource();
			var name = remoteName;

			Action<int> setProgress = progress =>
			{
				cancelled = !Progress.SetProgress(localName, name, progress);
				if (cancelled) cancellationToken.Cancel();
			};

			try
			{
				directory.UploadFile(localName, newFolderName, copyFlags, cancellationToken.Token, setProgress);
			}
			catch (OperationCanceledException ex)
			{
				//remove half uploaded file
				if (Root.Instance.TryGetItemByPath(remoteName, out remoteBlob))
					remoteBlob.Delete();
				return FileOperationResult.UserAbort;
			}

			if ((copyFlags & CopyFlags.Move) == CopyFlags.Move)
			{
				File.Delete(localName);
			}

			Progress.SetProgress(remoteName, localName, 100);
			return FileOperationResult.OK;
		}

		public override void StatusInfo(string remoteName, StatusOrigin origin, StatusOperation operation)
		{
			if (operation == StatusOperation.Delete && origin == StatusOrigin.Start)
			{
				deletionState = DeletionState.Initiated;
			}
		}

		public override FileOperationResult FileCopy(string source, string target, bool overwrite, bool move, RemoteInfo ri)
		{
			try
			{
				if (Root.Instance.GetBlobReferenceByTotalCmdPath(target).Exists())
					if (!Request.MessageBox(String.Format("The file '{0}' already exists.\n Do you want to owerwrite it?", target), MessageBoxButtons.YesNo))
						return FileOperationResult.OK;

				var src = Root.Instance.GetItemByPath(source);
				src.Copy(target);
				if (move)
					src.Delete();
				return FileOperationResult.OK;
			}
			catch (Exception ex)
			{
				OnError(ex);
				return FileOperationResult.WriteError;
			}
		}

		public override FileOperationResult DirectoryRename(string oldName, string newName, bool overwrite, RemoteInfo ri)
		{
			//TODO: Refactor, consider page blobs, move to BlobDirectory class?
			//TODO: Consider code reuse with Root.GetBlobReferenceByTotalCmdPath
			//TODO: rename container or storage account - handle;
			var targetParts = Regex.Split(newName, @"(^\\[^\\]*\\[^\\]*\\)");
			var targetContainer = targetParts[1];
			var targetDirectory = targetParts[2] + "/";

			var sourceParts = Regex.Split(oldName, @"(^\\[^\\]*\\[^\\]*\\)");
			var sourceDirectory = sourceParts[2].Replace('\\', '/') + "/";

			//Now let's fetch the blobs from "source folder"
			//TODO: use Directory.LoadAll - to fill the Tree.
			var blobs = Root.Instance.GetItemByPath<BlobDirectory>(oldName).CloudBlobDirectory.ListBlobs(true);
			//Now we'll enumerate through blobs
			foreach (var blob in blobs)
			{
				var sourceBlockBlob = blob as CloudBlockBlob;
				string newBlobName = targetDirectory + sourceBlockBlob.Name.Substring(sourceDirectory.Length);
				var newBlob = Root.Instance.GetItemByPath<BlobContainer>(targetContainer).CloudBlobContainer.GetBlockBlobReference(newBlobName);
				//TODO: reuse BlobItem.Copy 
				newBlob.StartCopyFromBlob(sourceBlockBlob);
				while (true)
				{
					//Since copy blob operation is an async operation, we must wait for the copy operation to finish.
					//To do so, we'll check if the copy operation is completed or not by fetching properties of the new blob.
					//TODO: make the same  for BlobItem.Copy
					newBlob.FetchAttributes();
					if (newBlob.CopyState.Status != CopyStatus.Pending)
					{
						break;
					}
					//It's still not completed. So wait for some time.
					System.Threading.Thread.Sleep(1000);
				}
				//Get the properties one more time
				newBlob.FetchAttributes();
				if (newBlob.CopyState.Status == CopyStatus.Success)
				{
					//Delete the source blob only if the copy is successful.
					sourceBlockBlob.DeleteIfExists();
				}
			}
			return FileOperationResult.OK;
		}

		public override bool FileRemove(string remoteName)
		{
			try
			{
				deletionState = DeletionState.None;
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
