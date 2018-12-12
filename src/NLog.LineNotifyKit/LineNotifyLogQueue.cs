using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NLog.LineNotifyKit
{
    public class LineNotifyLogQueue
    {
        internal static ConcurrentDictionary<int, StrongBox<int>> Counter = new ConcurrentDictionary<int, StrongBox<int>>();

        public static async Task<bool> WaitAsyncCompleted(int timeoutOfSecond = 30)
        {
            var processId = Process.GetCurrentProcess().Id;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!Counter.ContainsKey(processId)) return false;

            while (Counter[processId].Value > 0)
            {
                if (stopwatch.Elapsed > TimeSpan.FromSeconds(timeoutOfSecond))
                {
                    Counter.TryRemove(processId, out StrongBox<int> _);
                    return false;
                }

                await Task.Delay(100);
            }

            stopwatch.Stop();

            return true;
        }
    }
}
