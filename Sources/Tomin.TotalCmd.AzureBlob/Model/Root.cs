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
	public class Root : FileSystemItem
	{
		private const string AddNewStorageText = "<Add New Storage...>.";
		private Dictionary<string, CloudBlobClient> blobClients = new Dictionary<string, CloudBlobClient>();
		private static Root instance = new Root();

		private Root()
			: base(String.Empty, null, null)
		{

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
		protected override Task<IEnumerable<FileSystemItem>> LoadChildrenInternalAsync()
		{
			ReinitializeBlobClients();

			return Task.FromResult(
				blobClients.Select(x => new StorageAccount(x.Key, this, x.Value))
				.Concat<FileSystemItem>(new[] { 
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

		public FileSystemItem GetItemByPath(string path)
		{
			if (path == "\\")
				return instance;
			var levels = path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

			FileSystemItem currentItem = instance;
			foreach (string level in levels)
			{
				currentItem = currentItem[level];
				if (currentItem == null)
					throw new Exception(string.Format("Invalid operation: Node {0} is missing in path '{1}' ", level, path));
			}

			return currentItem;
		}
	}
}
