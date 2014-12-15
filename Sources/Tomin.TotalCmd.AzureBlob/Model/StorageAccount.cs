using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomin.TotalCmd.AzureBlob.Configuration;
using Tomin.TotalCmd.AzureBlob.Helpers;
using TotalCommander.Plugin.Wfx;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	internal class StorageAccount : FileSystemItemBase
	{
		public StorageAccount(string name, FileSystemItemBase parent, CloudBlobClient blobClient): base(name, parent)
		{
			BlobClient = blobClient;
			//Allow cancellation for Blob operations;
			BlobClient.DefaultRequestOptions.RetryPolicy = new RetryPolicyCancellationWrapper(BlobClient.DefaultRequestOptions.RetryPolicy);
		}

		public override bool IsFolder
		{
			get { return true; }
		}

		public CloudBlobClient BlobClient { get; private set; }

		/// <summary>
		/// returns Blob Containers
		/// </summary>
		/// <returns></returns>
		protected override async Task <IEnumerable<FileSystemItemBase>> LoadChildrenInternalAsync()
		{
			return await Task.Run(() => 
				BlobClient.ListContainers().Select( 
					c=> new BlobContainer(c.Name, this, c, c.Properties.LastModified) )
			);
			
		}

		public override bool CreateDirectory(string containerName)
		{
			//create container
			var newContainer = BlobClient.GetContainerReference(containerName);
			newContainer.CreateIfNotExists();
			return true;
		}

		public override void Delete()
		{
			StorageAccountsConfig.RemoveConfig(Name);
		}
	}
}
