using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalCommander.Plugin.Wfx;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	internal class StorageAccount : FileSystemItem
	{
		public StorageAccount(string name, FileSystemItem parent, CloudBlobClient blobClient): base(name, parent)
		{
			BlobClient = blobClient;
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
		protected override async Task <IEnumerable<FileSystemItem>> LoadChildrenInternalAsync()
		{
			return await Task.Run(() => 
				BlobClient.ListContainers().Select( 
					c=> new BlobContainer(c.Name, this, BlobClient, c.Properties.LastModified) )
			);
			
		}

	}
}
