using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	internal class BlobContainer : StorageAccount
	{
		public BlobContainer(string name, FileSystemItem parent, CloudBlobClient blobClient, DateTimeOffset? lastWriteTime)
			: base(name, parent, blobClient)
		{
			if (lastWriteTime != null)
				LastWriteTime = lastWriteTime.Value.ToLocalTime().DateTime;

			CloudBlobContainer = blobClient.GetContainerReference(name);
		}

		public CloudBlobContainer CloudBlobContainer { get; private set; }

		protected override async System.Threading.Tasks.Task<IEnumerable<FileSystemItem>> LoadChildrenInternalAsync()
		{
			BlobContinuationToken continuationToken = null;
			List<IListBlobItem> blobs = new List<IListBlobItem>();
			do
			{
				var listingResult = await CloudBlobContainer.ListBlobsSegmentedAsync(continuationToken);
				continuationToken = listingResult.ContinuationToken;
				blobs.AddRange(listingResult.Results);
			}
			while (continuationToken != null);

			List<FileSystemItem> resultItems = new List<FileSystemItem>();
			foreach (IListBlobItem blob in blobs)
			{
				if (blob is CloudBlobDirectory)
					resultItems.Add(
						new BlobDirectory(Uri.UnescapeDataString(blob.Uri.Segments.Last().TrimEnd('/')),this, BlobClient));
				else
					resultItems.Add(
						new BlobItem(Uri.UnescapeDataString(blob.Uri.Segments.Last().TrimEnd('/')),this, BlobClient));
			}

			return resultItems;
		}
	}
}
