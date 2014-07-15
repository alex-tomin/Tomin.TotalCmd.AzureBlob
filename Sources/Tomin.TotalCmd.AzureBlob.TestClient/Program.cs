using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomin.TotalCmd.AzureBlob.TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
			IEnumerator en;
			var data = new AzureBlobWfxPlugin().FindFirst("\\", out en);

			Console.WriteLine(data.FileName);
		}
	}
}
