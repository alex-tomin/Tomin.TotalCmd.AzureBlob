using System;

namespace TotalCommander.Plugin
{
    /// <summary>
    /// Parent window which can be used for showing a property sheet.
    /// </summary>
	public class TotalCommanderWindow
	{
        /// <summary>
        /// Winfow handle.
        /// </summary>
		public IntPtr Handle
		{
			get;
			private set;
		}

		internal TotalCommanderWindow(IntPtr handle)
		{
			Handle = handle;
		}


        /// <summary>
        /// Forces the window to invalidate its client area and immediately redraw 
        /// itself and any child controls.
        /// </summary>
        public void Refresh()
        {
            Win32.PostMessage(Handle, 1024 + 51, (IntPtr)540, IntPtr.Zero);
        }
    }
}
