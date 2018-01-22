using System;
using System.Collections.Generic;
using System.Text;

namespace AsynchronousProgramming.Standard.Library
{
    using System.Threading;

    /// <summary>
    /// 示例 ManualResetEventSlim
    /// </summary>
    public class ManualResetEventSlim_Test
    {
        public static void Test()
        {
            var manualSlim = new ManualResetEventSlim(false);
            Console.WriteLine("ManualResetEventSlim示例开始");
            Thread thread1 = new Thread(o =>
            {
                Thread.Sleep(500);
                Console.WriteLine("调用ManualResetEventSlim的Set()");
                manualSlim.Set();
            });
            thread1.Start();
            Console.WriteLine("调用ManualResetEventSlim的Wait()");
            manualSlim.Wait();
            Console.WriteLine("调用ManualResetEventSlim的Reset()");
            manualSlim.Reset();  // 重置为非终止状态，以便下一次Wait()等待
            var cts = new CancellationTokenSource();
            var thread2 = new Thread(obj =>
            {
                Thread.Sleep(500);
                var curCTS = obj as CancellationTokenSource;
                Console.WriteLine("调用CancellationTokenSource的Cancel()");
                curCTS.Cancel();
            });
            thread2.Start(cts);
            try
            {
                Console.WriteLine("调用ManualResetEventSlim的Wait()");
                manualSlim.Wait(cts.Token);
                Console.WriteLine("调用CancellationTokenSource后的输出");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("异常：OperationCanceledException");
            }
            manualSlim.Dispose();

            Console.Read();
        }
    }
}
