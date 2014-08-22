using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin;
using Tomin.TotalCmd.AzureBlob.Forms;
using System.Windows.Forms;
using Tomin.TotalCmd.AzureBlob.Configuration;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	public class NewAccountSupportFile : FileSystemItem
	{
		/// <summary>
		/// Initializes a new instance of the NewAccountSupportFile class.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="name"></param>
		public NewAccountSupportFile(string name, FileSystemItem parent)
			: base(name, parent)
		{ }
		public override bool IsFolder
		{
			get { return false; }
		}

		protected override Task<IEnumerable<FileSystemItem>> LoadChildrenInternalAsync()
		{
			throw new InvalidOperationException("File cannot be enumerated");
		}

		public override ExecuteResult ExecuteOpen(TotalCommanderWindow window, ref string remoteName)
		{
			//adding new storage account
			var addStorageDialog = new AddStorageDialog();
			if (addStorageDialog.ShowDialog() != DialogResult.OK)
				return ExecuteResult.OK;

			var blobConfig = addStorageDialog.StorageConfigInfo;
			StorageAccountsConfig.AddConfig(blobConfig);

			//redirect to Root folder, so it reflects the newly added folder
			remoteName = @"\";
			return ExecuteResult.SymLink;
		}
	}
}
