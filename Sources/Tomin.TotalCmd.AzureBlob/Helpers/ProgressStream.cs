using System;
using System.ComponentModel;

using System.IO;

namespace Tomin.TotalCmd.AzureBlob.Helpers
{
	public class ProgressStream : Stream
	{
		private Stream stream;
		private long bytesTransferred;
		private long totalLength;

		public event EventHandler<ProgressChangedEventArgs> ProgressChanged;



		public override void SetLength(long value)
		{
			totalLength = value;
			//this.stream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int result = stream.Read(buffer, offset, count);
			bytesTransferred += result;
			OnProgressChanged(bytesTransferred, totalLength);

			return result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.stream.Write(buffer, offset, count);
			bytesTransferred += count;
			OnProgressChanged(bytesTransferred, totalLength);
		}

		protected virtual void OnProgressChanged(long bytesTransferred, long totalLength)
		{
			if (ProgressChanged != null)
			{
				var ev = ProgressChanged;
				ev(this, new ProgressChangedEventArgs(bytesTransferred, totalLength));
			}
		}

		#region Transparent Wrapper
		public ProgressStream(Stream file, long? length = null)
		{
			this.stream = file;
			this.totalLength = length ?? file.Length;
			this.bytesTransferred = 0;
		}

		public override bool CanRead
		{
			get
			{
				return this.stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.stream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.stream.CanWrite;
			}
		}

		public override void Flush()
		{
			this.stream.Flush();
		}

		public override void Close()
		{
			this.stream.Close();
		}

		public override long Length
		{
			get
			{
				return this.stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.stream.Position;
			}
			set
			{
				this.stream.Position = value;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.stream.Seek(offset, origin);
		}


		protected override void Dispose(bool disposing)
		{
			stream.Dispose();
			base.Dispose(disposing);
		}
		#endregion

	}

	public class ProgressChangedEventArgs : EventArgs
	{
		public ProgressChangedEventArgs(long bytesRead, long totalLength)
		{
			this.BytesRead = bytesRead;
			this.TotalLength = totalLength;
		}

		public long BytesRead { get; set; }

		public long TotalLength { get; set; }

		public int Progress
		{
			get
			{
				if (TotalLength == 0)
					return 100;
				return (int)(BytesRead * 100 / TotalLength);
			}
		}
	}
}
