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
    public partial class ContainerPropertiesDialog : Form
    {

        private ContainerPropertiesDialog()
        {
            InitializeComponent();
        }

        public static void Edit(CloudBlobContainer cloudBlobContainer)
        {
            using (var dlg = new ContainerPropertiesDialog())
            {
                dlg.PublicAccess = cloudBlobContainer.GetPermissionsAsync().Result.PublicAccess;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var permissions = cloudBlobContainer.GetPermissionsAsync().Result;
                    permissions.PublicAccess = dlg.PublicAccess;
                    cloudBlobContainer.SetPermissions(permissions);
                }
            }
        }

        private BlobContainerPublicAccessType PublicAccess
        {
            get
            {
                switch (publicAccessComboBox.SelectedIndex)
                {
                    case   0:
                        return BlobContainerPublicAccessType.Off;
                    case   1:
                        return BlobContainerPublicAccessType.Container;
                    case   2:
                        return BlobContainerPublicAccessType.Blob;
                    default:
                        throw new ArgumentOutOfRangeException("publicAccessComboBox.SelectedIndex");
                }
            }
            set 
            {
                switch (value)
                {
                    case BlobContainerPublicAccessType.Off:
                        publicAccessComboBox.SelectedIndex = 0;
                        break;
                    case BlobContainerPublicAccessType.Container:
                        publicAccessComboBox.SelectedIndex = 1;
                        break;
                    case BlobContainerPublicAccessType.Blob:
                        publicAccessComboBox.SelectedIndex = 2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }
        }
    }
}
