using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AsynchronousProgramming.Standard.Library
{
    /// <summary>
    /// 双检锁
    /// 将一个实例对象的构造推迟到一个应用程序首次请求这个对象的时候进行
    /// </summary>
    public sealed class Singleton
    {
        private static object s_lock = new object();
        private static Singleton s_value = null;

        // 私有构造器，阻止这个类外部的任何代码创建实例
        private Singleton() { }

        public static Singleton GetSingleton()
        {
            if (s_value != null)
            {
                return s_value;
            }

            Monitor.Enter(s_lock);
            if (s_value == null)
            {
                s_value = new Singleton();
                // 无需使用Interlocked.Exchange，因为是先执行完构造函数再将引用返回给变量
                // 验证代码见 ValidateSingleton
                // Singleton temp = new Singleton();
                // Interlocked.Exchange(ref s_value, temp);
            }
            Monitor.Exit(s_lock);
            return s_value;
        }
    }

    /// <summary>
    /// 一个线程创建实例，并在构造函数中加入耗时操作Thread.Spin(Int32.MaxValue)，
    /// 一个线程不断访问s_value。得出结果是执行完构造函数后才会将变量引用返回。
    /// </summary>
    public class ValidateSingleton
    {
        private static Singleton s_value = null;
        public static void ValidateSingleton_Test()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                s_value = Singleton.GetSingleton();
            });

            ThreadPool.QueueUserWorkItem((state) =>
            {
                while (true)
                {
                    if (s_value != null)
                    {
                        Console.WriteLine("s_lock不为null");
                        break;
                    }
                }
            });

            Console.Read();
        }

        public sealed class Singleton
        {
            private static Object s_lock = new object();

            // 私有构造器，阻止这个类外部的任何代码创建实例
            private Singleton()
            {
                Thread.SpinWait(Int32.MaxValue);
                Console.WriteLine("对象创建完成");
            }

            public static Singleton GetSingleton()
            {
                if (s_value != null) return s_value;

                Monitor.Enter(s_lock);
                if (s_value == null)
                {
                    s_value = new Singleton();
                    Console.WriteLine("赋值完成");
                }
                Monitor.Exit(s_lock);
                return s_value;
            }
        }
    }
}
