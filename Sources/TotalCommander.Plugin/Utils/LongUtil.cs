using System;

namespace TotalCommander.Plugin.Utils
{
    static class LongUtil
    {
        public static int High(long @long)
        {
            return (int)(@long >> 32);
        }

        public static int Low(long @long)
        {
            return High(@long << 32);
        }

        public static long MakeLong(int high, int low)
        {
            return ((long)high << 32) + low;
        }
    }
}
