using System;
using System.Runtime.InteropServices.ComTypes;

namespace TotalCommander.Plugin.Utils
{
    static class DateTimeUtil
    {
        public static FILETIME ToFileTime(DateTime? dateTime)
        {
            var longTime = dateTime.HasValue ? dateTime.Value.ToFileTime() : long.MaxValue << 1;
            return new FILETIME()
            {
                dwHighDateTime = LongUtil.High(longTime),
                dwLowDateTime = LongUtil.Low(longTime),
            };
        }

        public static DateTime? FromFileTime(FILETIME fileTime)
        {
            var longTime = LongUtil.MakeLong(fileTime.dwHighDateTime, fileTime.dwLowDateTime);
            return longTime != 0 ? DateTime.FromFileTime(longTime) : (DateTime?)null;
        }

        public static int ToArchiveHeaderTime(DateTime d)
        {
            return (1980 <= d.Year && d.Year <= 2100) ? (d.Year - 1980) << 25 | d.Month << 21 | d.Day << 16 | d.Hour << 11 | d.Minute << 5 | d.Second / 2 : 0;
        }
    }
}