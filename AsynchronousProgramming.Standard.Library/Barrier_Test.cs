using System;
using System.Collections.Generic;
using System.Text;

namespace AsynchronousProgramming.Standard.Library
{
    using System.Threading;

    /// <summary>
    /// 示例 Barrier
    /// </summary>
    public class Barrier_Test
    {
        private static int m_count = 3;
        private static int m_curCount = 0;
        private static Barrier pauseBarr = new Barrier(2);

        public static void Test()
        {
            Thread.VolatileWrite(ref m_curCount, 0);
            var barr = new Barrier(m_count, new Action<Barrier>(Write_PhaseNumber));
            Console.WriteLine("Barrier开始第一阶段");
            AsyncSignalAndWait(barr, m_count);

            // 暂停等待 barr 第一阶段执行完毕
            pauseBarr.SignalAndWait();

            Console.WriteLine("Barrier开始第二阶段");
            Thread.VolatileWrite(ref m_curCount, 0);
            AsyncSignalAndWait(barr, m_count);

            // 暂停等待 barr 第二阶段执行完毕
            pauseBarr.SignalAndWait();

            pauseBarr.Dispose();
            barr.Dispose();
            Console.WriteLine("Barrier两个阶段执行完毕");

            Console.Read();
        }
        // 执行 SignalAndWait 方法
        private static void AsyncSignalAndWait(Barrier barr, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    Thread.Sleep(200);
                    Interlocked.Increment(ref m_curCount);
                    barr.SignalAndWait();
                });
            }
        }
        // 输出当前Barrier的当前阶段
        private static void Write_PhaseNumber(Barrier b)
        {
            Console.WriteLine(String.Format("Barrier调用完{0}次SignalAndWait()", m_curCount));
            Console.WriteLine("阶段编号为：" + b.CurrentPhaseNumber);
            Console.WriteLine("ParticipantsRemaining属性值为：" + b.ParticipantsRemaining);
            pauseBarr.SignalAndWait();
        }
    }
}
