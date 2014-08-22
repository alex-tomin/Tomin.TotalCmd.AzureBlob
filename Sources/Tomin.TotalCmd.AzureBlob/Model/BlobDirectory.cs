using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	internal class BlobDirectory : StorageAccount
	{
		public BlobDirectory(string name, FileSystemItem parent, CloudBlobClient blobClient)
			: base(name, parent, blobClient)
		{

		}

		public override bool IsFolder
		{
			get { return true; }
		}

		protected override async System.Threading.Tasks.Task<IEnumerable<FileSystemItem>> LoadChildrenInternalAsync()
		{
			throw new Exception("not impl");
		}

	}
}
