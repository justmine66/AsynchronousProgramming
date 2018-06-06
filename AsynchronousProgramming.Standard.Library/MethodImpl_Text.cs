using System;
using System.Collections.Generic;
using System.Text;

namespace AsynchronousProgramming.Standard.Library
{
    using System.Threading;
    using System.Runtime.CompilerServices;
    public class MethodImpl_Text
    {
        public static void Test()
        {
            class1 c = new class1();
            ThreadPool.QueueUserWorkItem(o => { c.Instance_Test1(); });
            Thread.Sleep(100);
            ThreadPool.QueueUserWorkItem(o => { c.Instance_Test2(); });

            ThreadPool.QueueUserWorkItem(o => { c.Instance_Test3(); });
            Thread.Sleep(2000);
            Console.WriteLine();
            Console.WriteLine();
            ThreadPool.QueueUserWorkItem(o => { class1.Static_Test1(); });
            Thread.Sleep(100);
            ThreadPool.QueueUserWorkItem(o => { class1.Static_Test2(); });

            ThreadPool.QueueUserWorkItem(o => { class1.Static_Test3(); });
            Console.Read();
        }

        internal class class1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            public void Instance_Test1()
            {
                Thread.Sleep(1000);
                Console.WriteLine("MethodImpl特性标注的实例方法----1");
                Console.WriteLine("1秒后释放lock (this)");
            }

            public void Instance_Test2()
            {
                lock (this)
                {
                    Console.WriteLine("MethodImpl使用Losk的实例方法----2");
                }
            }

            public void Instance_Test3()
            {
                Console.WriteLine("MethodImpl的实例方法----3");
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public static void Static_Test1()
            {
                Thread.Sleep(1000);
                Console.WriteLine("MethodImpl特性标注的静态方法----1");
                Console.WriteLine("1秒后释放lock (typeof(class1))");
            }

            public static void Static_Test2()
            {
                lock (typeof(class1))
                {
                    Console.WriteLine("MethodImpl使用Lock的静态方法----2");
                }
            }

            public static void Static_Test3()
            {
                Console.WriteLine("MethodImpl的静态方法----3");
            }
        }
    }
}
