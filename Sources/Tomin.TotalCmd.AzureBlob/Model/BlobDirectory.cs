using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	internal class BlobDirectory : FileSystemItemBase
	{
		public const string FakeFileName = "11FakeEmptyFile11";

		public BlobDirectory(string name, FileSystemItemBase parent, CloudBlobDirectory cloudBlobDirectory)
			: base(name, parent)
		{
			CloudBlobDirectory = cloudBlobDirectory;
		}

		public override bool IsFolder
		{
			get { return true; }
		}

		public CloudBlobDirectory CloudBlobDirectory { get; private set; }

		protected override async System.Threading.Tasks.Task<IEnumerable<FileSystemItemBase>> LoadChildrenInternalAsync()
		{
			BlobContinuationToken continuationToken = null;
			List<IListBlobItem> blobs = new List<IListBlobItem>();
			do
			{
				var listingResult = await CloudBlobDirectory.ListBlobsSegmentedAsync(continuationToken);
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
			var blob = CloudBlobDirectory.GetBlockBlobReference(String.Format("{0}/{1}", folderName, BlobDirectory.FakeFileName));
			blob.UploadText(string.Empty);
			return true;
		}

	}
}
