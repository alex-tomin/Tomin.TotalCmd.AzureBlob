using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	abstract class FileSystemItem
	{
		private IEnumerable<FileSystemItem> childrenList;

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler ParametersLoaded;

		/// <summary>
		/// Fires when next chunk of files are ready
		/// </summary>
		public event EventHandler ChildrenLoaded;

		public IEnumerable<FileSystemItem> Children
		{
			get
			{
				if (childrenList == null || !childrenList.Any())
					return LoadChildren();

				return childrenList;
			}
		}

		private IEnumerable<FileSystemItem> LoadChildren()
		{
			var result = LoadChildrenInternal();
			RaiseChildrenLoaded();
			return result;
		}

		protected abstract IEnumerable<FileSystemItem> LoadChildrenInternal();
		

		protected virtual void RaiseChildrenLoaded()
		{
			var eventCopy = ChildrenLoaded;
			if (eventCopy != null)
			{
				eventCopy(this, new EventArgs());
			}
		}

		protected virtual void RaiseParametersLoaded()
		{
			var eventCopy = ParametersLoaded;
			if (eventCopy != null)
			{
				eventCopy(this, new EventArgs());
			}
		}


	}
}
