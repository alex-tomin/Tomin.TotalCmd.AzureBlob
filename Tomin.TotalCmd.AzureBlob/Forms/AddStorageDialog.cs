using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tomin.TotalCmd.AzureBlob.Configuration;

namespace Tomin.TotalCmd.AzureBlob.Forms
{
	public partial class AddStorageDialog : Form
	{
		public AddStorageDialog()
		{
			InitializeComponent();
		}

		public BlobConfig StorageConfigInfo
		{
			get
			{
				return new BlobConfig
				{
					StorageDisplayName = displayNameTextBox.Text,
					AccountName = accountNameTextBox.Text,
					AccountKey = accountKeyTextBox.Text,
					UseSsl = useSslCheckBox.Checked,
					UseDevelopmentStorage = developmentStorageCheckBox.Checked
				};
			}
		}

		private void accountNameTextBox_Leave(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(displayNameTextBox.Text))
				displayNameTextBox.Text = accountNameTextBox.Text;
		}

		private void developmentStorageCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			accountNameTextBox.Enabled = accountKeyTextBox.Enabled
				= useSslCheckBox.Enabled = displayNameTextBox.Enabled
				= !developmentStorageCheckBox.Checked;

			displayNameTextBox.Text = developmentStorageCheckBox.Checked ? "(Development)" : accountNameTextBox.Text;

		}

		private void TextBox_Validating(object sender, CancelEventArgs e)
		{
			if (sender is TextBox)
				if (string.IsNullOrEmpty(((TextBox)sender).Text))
					errorProvider.SetError((Control)sender, "Field cannot be empty. Please fill the value");
				else
					errorProvider.SetError((Control)sender, "");
		}


	}
}
