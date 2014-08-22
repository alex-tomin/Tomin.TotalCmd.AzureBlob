using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	abstract public class FileSystemItem
	{
		private Dictionary<string, FileSystemItem> childrenDictionary = new Dictionary<string, FileSystemItem>();

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler ParametersLoaded;

		/// <summary>
		/// Fires when next chunk of files are ready
		/// </summary>
		public event EventHandler ChildrenLoaded;

		/// <summary>
		/// Initializes a new instance of the FileSystemItem class.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="name"></param>
		public FileSystemItem(string name, FileSystemItem parent, DateTime? LastWriteTime = null)
		{
			Parent = parent;
			Name = name;
			LastWriteTime = null;
		}


		public FileSystemItem this[string name]
		{
			get
			{
				if (!childrenDictionary.ContainsKey(name))
					return null;
				return childrenDictionary[name];
			}
		}

		public FileSystemItem Parent { get; private set; }

		public IEnumerable<FileSystemItem> Children
		{
			get
			{
				if (!childrenDictionary.Any())
					LoadChildren();

				//TODO: errorHandling;
				var ignoreMe = LoadChildrenAsync();

				return childrenDictionary.Values;
			}
		}

		public string Name { get; private set; }

		public DateTime? LastWriteTime { get; protected set; }

		public long FileSize { get; protected set; }

		public abstract bool IsFolder { get; }


		private async Task LoadChildrenAsync()
		{
			var updatedList = await LoadChildrenInternalAsync();
			RebindChildren(updatedList);
			RaiseChildrenLoaded();
		}

		private void LoadChildren()
		{
			var updatedList = LoadChildrenInternalAsync().Result;
			RebindChildren(updatedList);
		}

		private void RebindChildren(IEnumerable<FileSystemItem> newChildren)
		{
			foreach (FileSystemItem newItem in newChildren)
			{
				var correspondingOldItem = this[newItem.Name];
				if (correspondingOldItem != null)
					newItem.childrenDictionary = correspondingOldItem.childrenDictionary;
			}

			childrenDictionary = newChildren.ToDictionary(i => i.Name);
		}

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

		public virtual FindData ToFindData()
		{
			var findData = new FindData(Name, FileSize);
			findData.LastWriteTime = LastWriteTime;
			if (IsFolder)
				findData.Attributes = System.IO.FileAttributes.Directory;
			return findData;
		}

		protected abstract Task<IEnumerable<FileSystemItem>> LoadChildrenInternalAsync();

		#region TotalCmd Plugin Functions

		public virtual ExecuteResult ExecuteOpen(TotalCommanderWindow window, ref string remoteName)
		{
			return ExecuteResult.YourSelf;
		}
		#endregion
	}
}
