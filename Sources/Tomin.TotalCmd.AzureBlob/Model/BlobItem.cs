using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TotalCommander.Plugin.Wfx;
using System.Threading.Tasks;
using Tomin.TotalCmd.AzureBlob.Helpers;
using Microsoft.WindowsAzure.Storage;
using System.Threading;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	internal class BlobItem : FileSystemItemBase
	{
		public BlobItem(string name, FileSystemItemBase parent, ICloudBlob blob)
			: base(name, parent)
		{
			if (blob.Properties.LastModified != null)
				LastWriteTime = blob.Properties.LastModified.Value.ToLocalTime().DateTime;
			
			FileSize = blob.Properties.Length;
			CloudBlob = blob;
			CloudBlob.ServiceClient.DefaultRequestOptions.RetryPolicy = new RetryPolicyCancellationWrapper(CloudBlob.ServiceClient.DefaultRequestOptions.RetryPolicy);
		}


		public override bool IsFolder
		{
			get { return false; }
		}

		public ICloudBlob CloudBlob { get; private set; }

		protected override async Task<IEnumerable<FileSystemItemBase>> LoadChildrenInternalAsync()
		{
			throw new InvalidOperationException("Operation not supported on File items");
		}

		public override void DownloadFile(string remoteName, string localName, CopyFlags copyFlags, CancellationToken cancellationToken, Action<int> setProgress)
		{
			FileStream localFileStream = File.OpenWrite(localName);
			long length = CloudBlob.Properties.Length;

			using (var progressFileStream = new ProgressStream(localFileStream, length, cancellationToken))
			{
				progressFileStream.ProgressChanged += (sender, e) => setProgress(e.Progress);
				CloudBlob.DownloadToStream(progressFileStream);
			}
		}

		public override void Delete()
		{
			CloudBlob.Delete();
		}

	    public override void Copy(string target)
	    {
	        var targetCloudBlob = Root.Instance.GetBlobReferenceByTotalCmdPath(target);
            targetCloudBlob.StartCopyFromBlob(CloudBlob.Uri);
			//TODO: fetch and wait for copy state
	    }
	}
}