using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	internal class BlobItem : StorageAccount
	{
		public BlobItem(string name, FileSystemItem parent, CloudBlobClient blobClient)
			: base(name, parent, blobClient)
		{

		}

		public override bool IsFolder
		{
			get { return false; }
		}

		protected override async System.Threading.Tasks.Task<IEnumerable<FileSystemItem>> LoadChildrenInternalAsync()
		{
			throw new Exception("not impl");
		}

	}
}