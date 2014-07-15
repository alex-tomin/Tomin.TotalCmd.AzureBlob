using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace TotalCommander.Plugin
{
    static class Win32
    {
        public const int MAX_PATH = 1024;

        public const int ERROR_NO_MORE_FILES = 18;

        public static readonly IntPtr INVALID_HANDLE = new IntPtr(-1);


        [DllImport("kernel32")]
        public static extern void SetLastError(int errorCode);

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);


        public static string GetString(IntPtr ptr)
        {
            return ptr != IntPtr.Zero ? Marshal.PtrToStringUni(ptr) : string.Empty;
        }

        public static string[] GetStringArray(IntPtr ptr)
        {
            var result = new List<string>();
            if (ptr != IntPtr.Zero)
            {
                while (true)
                {
                    var str = Marshal.PtrToStringUni(ptr);
                    if (string.IsNullOrEmpty(str)) break;

                    result.Add(str);
                    ptr = new IntPtr(ptr.ToInt32() + 2 * (str.Length + 1));
                }
            }
            return result.ToArray();
        }

        public static void SetString(IntPtr ptr, string str)
        {
            SetString(ptr, str, MAX_PATH, Encoding.Unicode);
        }

        public static void SetString(IntPtr ptr, string str, int maxLen, Encoding encoding)
        {
            if (ptr == IntPtr.Zero) return;

            var i = 0;
            if (!string.IsNullOrEmpty(str))
            {
                var bytes = encoding.GetBytes(str);
                for (i = 0; i < Math.Min(bytes.Length, maxLen - 1); i++) Marshal.WriteByte(ptr, i, bytes[i]);
            }
            Marshal.WriteByte(ptr, i, 0);//null-terminated
        }
    }
}
