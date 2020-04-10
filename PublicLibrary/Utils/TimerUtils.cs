using System.Runtime.InteropServices;
namespace PublicLibrary.Utils
{
    public class TimerUtils
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);
        /// <summary>
        /// 获取当前时间的高精度值
        /// </summary>
        /// <returns></returns>
        public static long getHighPrecisionCurrentTime()
        {
            long currentTime;
            QueryPerformanceCounter(out currentTime);
            return currentTime;
        }

    }
}
