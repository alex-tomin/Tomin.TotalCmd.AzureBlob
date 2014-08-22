using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		}

		public override bool IsFolder
		{
			get { return false; }
		}

		protected override async System.Threading.Tasks.Task<IEnumerable<FileSystemItemBase>> LoadChildrenInternalAsync()
		{
			throw new Exception("not impl");
		}

			//		if (enumerator.Current is ICloudBlob)
			//{
			//	FindData findData = BlobToFindData((ICloudBlob)enumerator.Current);
			//	if (findData.FileName.Contains(FakeFileName))
			//		return FindNext(enumerator);
			//	return findData;
			//}


	}
}