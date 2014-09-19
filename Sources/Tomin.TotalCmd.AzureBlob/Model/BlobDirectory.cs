using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TotalCommander.Plugin.Wfx;
using System.Threading.Tasks;

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

		protected override async Task<IEnumerable<FileSystemItemBase>> LoadChildrenInternalAsync()
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
				string blobName = Uri.UnescapeDataString(blob.Uri.Segments.Last().TrimEnd('/'));

				if (blob is CloudBlobDirectory)
					resultItems.Add(new BlobDirectory(blobName, this, (CloudBlobDirectory)blob));
				else if (blob is ICloudBlob)
				{
					resultItems.Add(new BlobItem(blobName, this, (ICloudBlob)blob));
				}
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

		public override FileOperationResult UploadFile(string localName, string remoteName, CopyFlags copyFlags)
		{
			var blob = CloudBlobDirectory.GetBlockBlobReference(remoteName);
			blob.UploadFromFile(localName, FileMode.Open);
			return FileOperationResult.OK;
		}

		public override void Delete()
		{
#warning make delete operation async?
			BlobContinuationToken continuationToken = null;
			do
			{
				var listingResult = CloudBlobDirectory.ListBlobsSegmented(true, BlobListingDetails.None, 100, continuationToken, null, null);
				continuationToken = listingResult.ContinuationToken;
				Parallel.ForEach(
					listingResult.Results.OfType<ICloudBlob>(),
					c =>c.Delete()
				);
			}
			while (continuationToken != null);
		}

		public void LoadAllSubItems()
		{
			BlobContinuationToken continuationToken = null;
			List<IListBlobItem> blobs = new List<IListBlobItem>();
			do
			{
				var listingResult = CloudBlobDirectory.ListBlobsSegmented(true, BlobListingDetails.None, 100, continuationToken, null, null);
				continuationToken = listingResult.ContinuationToken;
				blobs.AddRange(listingResult.Results);
			}
			while (continuationToken != null);


			foreach (ICloudBlob blob in blobs.OfType<ICloudBlob>().OrderBy(x => x.Name))
			{
				throw new Exception("not implemented");
#warning - implement hierarchy
				//TODO: create hierarchy here
			}


			//b => new BlobItem(
			//	Uri.UnescapeDataString(b.Uri.Segments.Last().TrimEnd('/'))
			//	, this, b));
		}
	}
}