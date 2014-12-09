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
		enum UpdateOperationState
		{
			None = 0,
			Initiated,
			Enumerated
		}

		private static Dictionary<string, CloudBlobClient> blobClients = new Dictionary<string, CloudBlobClient>();
		private static Dictionary<string, DateTime> directoryLastWriteTimeCache = new Dictionary<string, DateTime>();
		private readonly TimeSpan cacheDuration = TimeSpan.FromSeconds(30); //Todo: move to config

		//TODO: multithreaded support
		private UpdateOperationState updateOperationState = UpdateOperationState.None;

		public AzureBlobWfxPlugin()
		{
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		}

		public override string PluginName { get { return "AzureBlob"; } }

		public override FindData FindFirst(string path, out IEnumerator enumerator)
		{
			//Debugger.Launch();

			var currentNode = Root.Instance.GetItemByPath(path);

			if (updateOperationState == UpdateOperationState.Initiated)
			{
				if (currentNode is BlobDirectory)
				{
					//get all files in a flat way
					((BlobDirectory)currentNode).LoadAllSubItems();
					updateOperationState = UpdateOperationState.Enumerated; //TotalCMD triggers enumeration twice, we want only once.
				}
				else if (currentNode is BlobItem)
				{
					//don't enumerate items - proceed to folder deletion immediately
					enumerator = Enumerable.Empty<FindData>().GetEnumerator();
					return FindNext(enumerator);
				}
				else
				{
                    currentNode.LoadChildren();
                    updateOperationState = UpdateOperationState.Enumerated;
				}
			}
			else if (updateOperationState == UpdateOperationState.None)
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
		    switch (operation)
		    {
		        case StatusOperation.PutSingle:
		        case StatusOperation.PutMulti:
		        case StatusOperation.RenameMoveSingle:
		        case StatusOperation.RenameMoveMulti:
		        case StatusOperation.Delete:
		        case StatusOperation.CreateDirectory:
		        case StatusOperation.SyncPut:
		        case StatusOperation.SyncDelete:
		        case StatusOperation.PutMultiThread:
                    if (origin == StatusOrigin.End)
                        updateOperationState = UpdateOperationState.Initiated;
                    break;
                default:
		            if (updateOperationState == UpdateOperationState.Enumerated)
		                updateOperationState = UpdateOperationState.None;
                    break;
		    }
		}

	    public override FileOperationResult FileCopy(string source, string target, bool overwrite, bool move, RemoteInfo ri)
	    {
	        try
	        {
	            if (Root.Instance.GetCloudBlobByPath(target).Exists())
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
			}}

	    public override FileOperationResult DirectoryRename(string oldName, string newName, bool overwrite, RemoteInfo ri)
	    {
	        return FileOperationResult.NotSupported;
	    }

	    public override bool FileRemove(string remoteName)
		{
			try
			{
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
