using System;
using System.Collections.Generic;
using System.Text;

namespace AsynchronousProgramming.Standard.Library
{
    using System.Threading;
    /// <summary>
    /// 线程池线程协作取消
    /// </summary>
    public class ThreadPool_Cancel
    {
        public static void ThreadPool_Cancel_test()
        {
            var cts = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(token =>
            {
                var curCancelToken = (CancellationToken)token;
                while (true)
                {
                    // 耗时操作
                    Thread.Sleep(400);
                    if (curCancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
                Console.WriteLine(String.Format("线程{0}上，CancellationTokenSource操作已取消，退出循环"
                        , Thread.CurrentThread.ManagedThreadId));
            }, cts.Token);

            ThreadPool.QueueUserWorkItem(token =>
                {
                    Console.WriteLine(String.Format("线程{0}上，调用CancellationToken实例的 WaitHandle.WaitOne() "
                        , Thread.CurrentThread.ManagedThreadId));
                    var curCancelToken = (CancellationToken)token;
                    curCancelToken.WaitHandle.WaitOne();
                    Console.WriteLine(String.Format("线程{0}上，CancellationTokenSource操作已取消，WaitHandle获得信号"
                        , Thread.CurrentThread.ManagedThreadId));
                }, cts.Token);

            Thread.Sleep(2000);
            Console.WriteLine("执行CancellationTokenSource实例的Cancel()");
            cts.Cancel();

            Console.Read();
        }
    }
}
