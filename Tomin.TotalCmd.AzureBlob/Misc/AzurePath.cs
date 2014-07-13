using System.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Tomin.TotalCmd.AzureBlob.Misc
{
	public class AzurePath
	{
		private static readonly Regex blobPathRegex = new Regex(@"\\(?<account>[^\\]+)\\?(?<container>[^\\]*)\\?(?<folder>.*)", RegexOptions.Compiled);

		public string StorageDisplayName { get; set; }

		public string ContainerName { get; set; }
		public string Path { get; set; }

		/// <summary>
		/// Path contains only Storage Account (Display Name) folder.
		/// </summary>
		public bool IsAccountOnly
		{
			get { return string.IsNullOrEmpty(ContainerName); }
		}

		/// <summary>
		/// Path is root for Azure Container, no futher path.
		/// </summary>
		public bool IsContainerOnly
		{
			get { return string.IsNullOrEmpty(Path) && !string.IsNullOrEmpty(ContainerName); }
		}

		public static AzurePath FromPath(string remoteName)
		{
			Match regexMatch = blobPathRegex.Match(remoteName);

			var azurePath = new AzurePath
						{
							ContainerName = regexMatch.Groups["container"].Value,
							StorageDisplayName = regexMatch.Groups["account"].Value,
							Path = regexMatch.Groups["folder"].Value.Replace('\\', '/')
						};
			return azurePath;
		}
	}
}
