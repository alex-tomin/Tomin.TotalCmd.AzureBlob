using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	internal class StorageAccount : FileSystemItem
	{
		public StorageAccount()
		{

		}

		protected override IEnumerable<FileSystemItem> LoadChildrenInternal()
		{
			throw new NotImplementedException();
		}
	}
}
