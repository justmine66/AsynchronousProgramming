using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousProgramming.Dotnet.ConsonleApp
{
    using System.Threading;
    using System.Runtime.Remoting.Contexts;

    /// <summary>
    /// 示例：特性Synchronization
    /// </summary>
    public class Synchronization_Test
    {
        public static void Test()
        {
            class1 c = new class1();
            ThreadPool.QueueUserWorkItem(o => { c.Test1(); });
            Thread.Sleep(100);
            ThreadPool.QueueUserWorkItem(o => { c.Test2(); });

            Console.Read();
        }

        [Synchronization(SynchronizationAttribute.REQUIRED)]
        internal class class1 : ContextBoundObject
        {
            public void Test1()
            {
                Thread.Sleep(1000);
                Console.WriteLine("Test1");
                Console.WriteLine("1秒后");
            }

            public void Test2()
            {
                Console.WriteLine("Test2");
            }
        }
    }
}
