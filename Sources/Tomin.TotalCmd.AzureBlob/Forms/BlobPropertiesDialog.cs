using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Tomin.TotalCmd.AzureBlob.Forms
{
    public partial class BlobPropertiesDialog : Form
    {

        private BlobPropertiesDialog()
        {
            InitializeComponent();
        }

        public static void Edit(ICloudBlob cloudBlob)
        {
            using (var dlg = new BlobPropertiesDialog())
            {
                dlg.contentTypeTextBox.Text = cloudBlob.Properties.ContentType;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    cloudBlob.Properties.ContentType = dlg.contentTypeTextBox.Text;
                    cloudBlob.SetProperties();
                }
            }
        }
    }
}
