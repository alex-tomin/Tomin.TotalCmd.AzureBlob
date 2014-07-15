using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomin.TotalCmd.AzureBlob.Configuration
{
	public class BlobConfig : IEquatable<BlobConfig>
	{
		public string StorageDisplayName { get; set; }

		public string AccountName { get; set; }

		public string AccountKey { get; set; }

		public bool UseSsl { get; set; }

		public bool UseDevelopmentStorage { get; set; }


		#region IEquatable<ValuesClass> Members

		public bool Equals(BlobConfig other)
		{
			if (ReferenceEquals(null, other)) return false;
			return (StorageDisplayName.Equals(other.StorageDisplayName));
		}

		#endregion

		public override bool Equals(object obj)
		{
			return this.Equals(obj as BlobConfig);
		}

		public override int GetHashCode()
		{
			return StorageDisplayName.GetHashCode();
		}
	}
}
