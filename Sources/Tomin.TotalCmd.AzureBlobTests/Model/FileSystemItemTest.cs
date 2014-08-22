using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomin.TotalCmd.AzureBlob.Model;
using Moq;
using Moq.Protected;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tomin.TotalCmd.AzureBlobTests.Model
{
	[TestClass]
	public class FileSystemItemTest
	{
		[TestMethod]
		public void RebindChildren_Success()
		{
			var root = new Mock<FileSystemItemBase>("", null, null);
			root.Protected().Setup<Task<IEnumerable<FileSystemItemBase>>>("LoadChildrenInternalAsync").Returns(Task.FromResult(
				new [] { 
							new Mock<FileSystemItemBase>("testName", root.Object, null).Object,
							new Mock<FileSystemItemBase>("testName1", root.Object, null).Object,
							new Mock<FileSystemItemBase>("testName2", root.Object, null).Object,
				}.AsEnumerable()
			));

			var p = root.Object.Children;

		}
	}
}
