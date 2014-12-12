using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace Tomin.TotalCmd.AzureBlob.Model
{
	abstract public class FileSystemItemBase
	{
		private Dictionary<string, FileSystemItemBase> childrenDictionary = new Dictionary<string, FileSystemItemBase>();
		DateTime lastLoadTime;

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
		public FileSystemItemBase(string name, FileSystemItemBase parent, DateTime? LastWriteTime = null)
		{
			Parent = parent;
			Name = name;
			LastWriteTime = null;
		}


		public FileSystemItemBase this[string name]
		{
			get
			{
				if (!childrenDictionary.Any())
					LoadChildren();

				if (!childrenDictionary.ContainsKey(name))
					LoadChildren();

				if (!childrenDictionary.ContainsKey(name))
					return null;
				return childrenDictionary[name];
			}
		}

		public FileSystemItemBase Parent { get; private set; }

		public IEnumerable<FileSystemItemBase> Children
		{
			get
			{
				if (!childrenDictionary.Any())
					LoadChildren();

#warning - redesign, calculate upfront;
				if (IsFolder)
					LastWriteTime = childrenDictionary.Values.Max(x => x.LastWriteTime);

				//TODO: use some attribute - like invisible file to make decupling
				return childrenDictionary.Values.Where(i=>i.Name != BlobDirectory.FakeFileName);
			}
		}


		public string Name { get; private set; }

		public DateTime? LastWriteTime { get; protected set; }

		public long FileSize { get; protected set; }

		public abstract bool IsFolder { get; }


		public async Task LoadChildrenAsync()
		{
			var updatedList = await LoadChildrenInternalAsync();
			RebindChildren(updatedList);
			RaiseChildrenLoaded();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cacheDuration">consequent load will be missed during this period</param>
		public void LoadChildren(TimeSpan? cacheDuration = null)
		{
			if (cacheDuration != null && DateTime.Now - lastLoadTime < cacheDuration)
				return;
			var updatedList = LoadChildrenInternalAsync().Result;
			RebindChildren(updatedList);
			lastLoadTime = DateTime.UtcNow;
		}

#warning review race conditions
		protected void RebindChildren(IEnumerable<FileSystemItemBase> newChildren)
		{
			foreach (FileSystemItemBase newItem in newChildren)
			{
				if (!childrenDictionary.ContainsKey(newItem.Name))
					continue;

				newItem.childrenDictionary = childrenDictionary[newItem.Name].childrenDictionary;
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

		protected abstract Task<IEnumerable<FileSystemItemBase>> LoadChildrenInternalAsync();

		#region TotalCmd Plugin Functions

		public virtual ExecuteResult ExecuteOpen(TotalCommanderWindow window, ref string remoteName)
		{
			return ExecuteResult.YourSelf;
		}

		public virtual bool CreateDirectory(string folderName)
		{
			if (!IsFolder)
				return false;
			throw new NotImplementedException("Creating sub directory is not yet implemented");
		}

		public virtual FileOperationResult DownloadFile(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri)
		{
			if (IsFolder)
				throw new InvalidOperationException("Invalid opertion on Folder");

			throw new NotImplementedException("Not Imlemented yet");
		}

		public virtual FileOperationResult UploadFile(string localName, string remoteName, CopyFlags copyFlags)
		{
			if (!IsFolder)
				throw new InvalidOperationException("Invalid opertion on File");

			throw new NotImplementedException("You cannot create files in this level. Select a subfolder");
		}

		public virtual void Delete()
		{
			throw new NotImplementedException("Delete operation is not implemented on this type");
		}

		#endregion

	    public virtual void Copy(string target)
	    {
	        throw new NotImplementedException("Copy operation is not implemented on this type");
	    }
	}
}
