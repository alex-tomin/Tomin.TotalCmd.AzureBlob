using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomin.TotalCmd.AzureBlob.Configuration
{
	[Serializable]
	public class BlobConfigSettings : ApplicationSettingsBase
	{

		// If you used [ApplicationScopedSetting()] instead of [UserScopedSetting()],
		// you would NOT be able to persist any data changes!
		[UserScopedSetting()]
		[SettingsSerializeAs(SettingsSerializeAs.Xml)]
		[DefaultSettingValue("")]
		public List<BlobConfig> BlobConfigs
		{
			get
			{
				return ((List<BlobConfig>)this["BlobConfigs"]);
			}
			set
			{
				this["BlobConfigs"] = (List<BlobConfig>)value;
			}
		}
	}
}
