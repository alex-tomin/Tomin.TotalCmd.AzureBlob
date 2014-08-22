using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tomin.TotalCmd.AzureBlob.Configuration
{
	class StorageAccountsConfig
	{
		private static readonly string SettingsFile = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StorageAccounts.config");
		static List<BlobConfig> storageAccountSettings = new List<BlobConfig>();
		static object storageAccountsLock = new object();

		public static IReadOnlyList<BlobConfig> StorageAccountSettings
		{
			get
			{
				if (storageAccountSettings == null || !storageAccountSettings.Any())
					ReLoadBlobConfigs();
				return storageAccountSettings.AsReadOnly();
			}
		}

		public static void AddConfig(BlobConfig blobConfig)
		{
			//monitor is reentrant - so this doesn't cause a deadlock
			lock (storageAccountsLock)
			{
				if (storageAccountSettings.Any(s => s.StorageDisplayName == blobConfig.StorageDisplayName))
					throw new Exception("Display Name already exists. Please give another name.");

				storageAccountSettings.Add(blobConfig);
				SaveBlobConfigs();
			}
		}

		public static void RemoveConfig(string configName)
		{
			lock (storageAccountsLock)
			{
				var itemToRemove = storageAccountSettings.SingleOrDefault(c => c.StorageDisplayName == configName);
				if (itemToRemove != null)
					storageAccountSettings.Remove(itemToRemove);
				SaveBlobConfigs();
			}
		}

		public static void ReLoadBlobConfigs()
		{
			lock (storageAccountsLock)
			{
				if (!File.Exists(SettingsFile))
				{
					storageAccountSettings = GetDefaultSettings().ToList();
					return;
				}

				var serializer = new XmlSerializer(storageAccountSettings.GetType());
				using (var file = new StreamReader(SettingsFile))
				{
					storageAccountSettings = (List<BlobConfig>)serializer.Deserialize(file);
				}
			}
		}

		public static void SaveBlobConfigs()
		{
			var serializer = new XmlSerializer(storageAccountSettings.GetType());
			using (var file = new StreamWriter(SettingsFile))
			{
				lock (storageAccountsLock)
				{
					serializer.Serialize(file, storageAccountSettings);
				}
			}
		}

		private static IEnumerable<BlobConfig> GetDefaultSettings()
		{
			return new[]{
				new BlobConfig { StorageDisplayName = "(Development)", UseSsl = true, UseDevelopmentStorage = true},
			};
		}

	}
}
