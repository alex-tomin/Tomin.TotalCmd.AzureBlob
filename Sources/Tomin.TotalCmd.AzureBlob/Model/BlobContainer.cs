using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalCommander.Plugin.Wfx;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	internal class BlobContainer : FileSystemItemBase
	{
		public BlobContainer(string name, FileSystemItemBase parent, CloudBlobContainer blobContainer, DateTimeOffset? lastWriteTime)
			: base(name, parent)
		{
			if (lastWriteTime != null)
				LastWriteTime = lastWriteTime.Value.ToLocalTime().DateTime;

			CloudBlobContainer = blobContainer;
		}

		public override bool IsFolder
		{
			get { return true; }
		}

		public CloudBlobContainer CloudBlobContainer { get; protected set; }

		protected override async System.Threading.Tasks.Task<IEnumerable<FileSystemItemBase>> LoadChildrenInternalAsync()
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

			List<FileSystemItemBase> resultItems = new List<FileSystemItemBase>();
			foreach (IListBlobItem blob in blobs)
			{
				if (blob is CloudBlobDirectory)
					resultItems.Add(
						new BlobDirectory(Uri.UnescapeDataString(blob.Uri.Segments.Last().TrimEnd('/')), this, (CloudBlobDirectory)blob));
				else if (blob is ICloudBlob)
					resultItems.Add(
						new BlobItem(Uri.UnescapeDataString(blob.Uri.Segments.Last().TrimEnd('/')), this, (ICloudBlob)blob));
				else
					throw new InvalidOperationException("Blob type is unknown");
			}

			return resultItems;
		}

		public override bool CreateDirectory(string folderName)
		{
			var blob = CloudBlobContainer.GetBlockBlobReference(String.Format("{0}/{1}", folderName, BlobDirectory.FakeFileName));
			blob.UploadText(string.Empty);
			return true;
		}

		public override FileOperationResult UploadFile(string localName, string remoteName, CopyFlags copyFlags)
		{
			var blob = CloudBlobContainer.GetBlockBlobReference(remoteName);
			blob.UploadFromFile(localName, FileMode.Open);
			return FileOperationResult.OK;
		}

		public override void Delete()
		{
			CloudBlobContainer.Delete();
		}
	}
}
