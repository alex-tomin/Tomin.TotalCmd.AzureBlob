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
	internal class BlobItem : FileSystemItemBase
	{
		public BlobItem(string name, FileSystemItemBase parent, ICloudBlob blob)
			: base(name, parent)
		{
			if (blob.Properties.LastModified != null)
				LastWriteTime = blob.Properties.LastModified.Value.ToLocalTime().DateTime;
			
			FileSize = blob.Properties.Length;
			CloudBlob = blob;
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

		public override FileOperationResult DownloadFile(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri)
		{
			using (var fileStream = File.OpenWrite(localName))
			{
				CloudBlob.DownloadToStream(fileStream);
			}

			return FileOperationResult.OK;
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