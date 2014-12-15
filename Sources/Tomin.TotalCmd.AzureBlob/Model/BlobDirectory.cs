using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TotalCommander.Plugin.Wfx;
using System.Threading.Tasks;
using Tomin.TotalCmd.AzureBlob.Helpers;
using System.Threading;

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

		public override void UploadFile(string localName, string remoteName, CopyFlags copyFlags, CancellationToken cancellationToken, Action<int> setProgress)
		{
			FileStream localFileStream = File.OpenRead(localName);
			var blob = CloudBlobDirectory.GetBlockBlobReference(remoteName);
			blob.ServiceClient.DefaultRequestOptions.RetryPolicy = new RetryPolicyCancellationWrapper(blob.ServiceClient.DefaultRequestOptions.RetryPolicy);

			using (var progressFileStream = new ProgressStream(localFileStream, cancellationToken:cancellationToken))
			{
				progressFileStream.ProgressChanged += (sender, e) => setProgress(e.Progress);
				blob.UploadFromStream(progressFileStream);
			}
		}

		public override void Delete()
		{
			BlobContinuationToken continuationToken = null;
			do
			{
				var listingResult = CloudBlobDirectory.ListBlobsSegmented(true, BlobListingDetails.None, 100, continuationToken, null, null);
				continuationToken = listingResult.ContinuationToken;
				Parallel.ForEach(
					listingResult.Results.OfType<ICloudBlob>(),
					c => c.Delete()
				);
			}
			while (continuationToken != null);
		}

		//TODO: make async;
		public void LoadAllSubItems()
		{
			BlobContinuationToken continuationToken = null;
			List<IListBlobItem> blobs = new List<IListBlobItem>();
			do
			{
				var listingResult = CloudBlobDirectory.ListBlobsSegmented(true, BlobListingDetails.None, 5000, continuationToken, null, null);
				continuationToken = listingResult.ContinuationToken;
				blobs.AddRange(listingResult.Results);
			}
			while (continuationToken != null);

			FillChildren(blobs.OfType<ICloudBlob>());
		}

		private void FillChildren(IEnumerable<ICloudBlob> blobList)
		{
			List<FileSystemItemBase> directChildren = new List<FileSystemItemBase>();
			var lookup = blobList.ToLookup(b =>
				{
					string subPath = b.Name.Substring(CloudBlobDirectory.Prefix.Length);
					int slashIndex = subPath.IndexOf('/');
					return slashIndex != -1 ? subPath.Substring(0, slashIndex) : string.Empty;
				});

			foreach (var folder in lookup)
			{
				string subDirName = folder.Key;
				if (string.IsNullOrEmpty(subDirName))
				{
					directChildren.AddRange(
						folder.Select(b => new BlobItem(
							Uri.UnescapeDataString(b.Uri.Segments.Last().TrimEnd('/')),
							this, b)));
				}
				else
				{
					var subDir = new BlobDirectory(subDirName, this, CloudBlobDirectory.GetDirectoryReference(subDirName));
					subDir.FillChildren(folder);
					directChildren.Add(subDir);
				}
			}

			RebindChildren(directChildren);

		}
	}
}
