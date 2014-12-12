using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomin.TotalCmd.AzureBlob.Configuration;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	public class Root : FileSystemItemBase
	{
		private const string AddNewStorageText = "<Add New Storage...>.";
		private Dictionary<string, CloudBlobClient> blobClients = new Dictionary<string, CloudBlobClient>();
		private static Root instance = new Root();

		private Root()
			: base(String.Empty, null, null)
		{
			LoadChildren();
		}

		public static Root Instance
		{
			get { return instance; }
		}

		public override bool IsFolder
		{
			get { return true; }
		}

		/// <summary>
		/// Returns Storage Accounts
		/// </summary>
		/// <returns></returns>
		protected override Task<IEnumerable<FileSystemItemBase>> LoadChildrenInternalAsync()
		{
			ReinitializeBlobClients();

			return Task.FromResult(
				blobClients.Select(x => new StorageAccount(x.Key, this, x.Value))
				.Concat<FileSystemItemBase>(new[] { 
					new NewAccountSupportFile(AddNewStorageText, this),
				}));
		}

		private void ReinitializeBlobClients()
		{
			blobClients = StorageAccountsConfig.StorageAccountSettings.ToDictionary(
						s => s.StorageDisplayName,
						s => s.UseDevelopmentStorage
							? CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient()
							: new CloudStorageAccount(new StorageCredentials(s.AccountName, s.AccountKey), s.UseSsl).CreateCloudBlobClient()
					);
		}

		public FileSystemItemBase GetItemByPath(string path)
		{
			if (path == "\\")
				return instance;
			var levels = path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

			FileSystemItemBase currentItem = instance;
			foreach (string level in levels)
			{
				currentItem = currentItem[level];
				if (currentItem == null)
					throw new Exception(String.Format("Invalid operation: Node {0} is missing in path '{1}' ", level, path));
			}

			return currentItem;

		}

		public override bool CreateDirectory(string folderName)
		{
			throw new NotImplementedException(String.Format("Please double click on special item below {0} to add a new account", AddNewStorageText));
		}

	    internal CloudBlockBlob GetCloudBlobByPath(string target)
	    {
	        var targetParts = Regex.Split(target, @"(^\\[^\\]*\\[^\\]*\\)");
	        var targetContainer = targetParts[1];
	        var targetBlob = targetParts[2];

	        var targetCloudBlob = GetItemByPath(targetContainer).CloudBlobContainer.GetBlockBlobReference(targetBlob);
	        return targetCloudBlob;
	    }
	}
}
