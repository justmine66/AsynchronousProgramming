using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AsynchronousProgramming.Standard.Library
{
    /// <summary>
    /// 带例子的Main
    /// </summary>
    public class SampledMain
    {
        #region [ 示例：使用帮助器类传递参数并使用回调函数检索数据 ]

        /// <summary>
        /// 示例：使用帮助器类传递参数并使用回调函数检索数据
        /// </summary>
        public static void SimpleCallBackWithParam()
        {
            // 将需传递给异步执行方法数据传递给帮助器类
            var tws = new ThreadWithState(
            "This report displays the number {0}.",
            42,
            new ThreadCallback(ResultCallback));

            var t = new Thread(new ThreadStart(tws.ThreadProc));
            t.Start();
            Console.WriteLine("Main thread does some work, then waits.");
            t.Join();
            Console.WriteLine("Independent task has completed; main thread ends.");

            Console.Read();
        }

        static void ResultCallback(int lineCount)
        {
            Console.WriteLine("Independent task printed {0} lines.", lineCount);
        }

        #endregion

        #region [ 示例：Abort()与ResetAbort()使用 ]

        /// <summary>
        /// 示例：使用Abort终止线程
        /// </summary>
        public static void SimpleAbort()
        {
            var autoReset = new AutoResetEvent(false);

            var t = new Thread((object autoResetParam) =>
            {
                try
                {
                    var autoResetInner = autoResetParam as AutoResetEvent;
                    Console.WriteLine("try内部，调用Abort前。");
                    autoResetInner.Set();
                    autoResetInner.WaitOne();
                    Console.WriteLine("try内部，调用Abort后。");

                }
                catch (ThreadAbortException abortEx)
                {
                    Console.WriteLine("catch:" + abortEx.GetType());
                    Thread.ResetAbort();
                    Console.WriteLine("catch：调用ResetAbort()。");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("catch：" + ex.GetType());
                }
                finally
                {
                    Console.WriteLine("finally");
                    //Thread.ResetAbort();
                    //Console.WriteLine("调用ResetAbort()。");
                }

                Console.WriteLine("try外面，调用Abort后（若再catch中调用了ResetAbort，则try块外面的代码依旧执行，即：线程没有终止）。");
            });
            t.Start(autoReset);
            autoReset.WaitOne();
            t.Abort();
            Console.WriteLine("主线程，调用Abort。");
            autoReset.Set();

            t.Join();

            Console.Read();
        }

        #endregion

        #region [ 示例：TLS[thread-local storage 托管线程本地存储区]（数据槽|线程相关静态字段）中数据的唯一性 ]

        /// <summary>
        /// 示例：TLS[thread-local storage 托管线程本地存储区]（数据槽|线程相关静态字段）中数据的唯一性
        /// </summary>
        public static void SimpleTLS()
        {
            Console.WriteLine("数据槽");
            TLS4DataSlot();

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("线程相关静态字段");
            TLS4StaticField();

            Console.Read();
        }

        /// <summary>
        /// 数据槽  的使用示例
        /// </summary>
        private static void TLS4DataSlot()
        {
            LocalDataStoreSlot slot = Thread.AllocateNamedDataSlot("Name");
            Console.WriteLine(String.Format("ID为{0}的线程，命名为\"Name\"的数据槽，开始设置数据。", Thread.CurrentThread.ManagedThreadId));

            Thread.SetData(slot, "小丽");
            Console.WriteLine(String.Format("ID为{0}的线程，命名为\"Name\"的数据槽，数据是\"{1}\"。", Thread.CurrentThread.ManagedThreadId, Thread.GetData(slot)));

            var newThread = new Thread(() =>
            {
                LocalDataStoreSlot storeSlot = Thread.GetNamedDataSlot("Name");
                Console.WriteLine(String.Format("ID为{0}的线程，命名为\"Name\"的数据槽，在新线程为其设置数据前为\"{1}\"。", Thread.CurrentThread.ManagedThreadId, Thread.GetData(storeSlot)));

                Console.WriteLine(String.Format("ID为{0}的线程，命名为\"Name\"的数据槽，开始设置数据。", Thread.CurrentThread.ManagedThreadId));
                Thread.SetData(storeSlot, "小红");
                Console.WriteLine(String.Format("ID为{0}的线程，命名为\"Name\"的数据槽，在新线程为其设置数据 后 为\"{1}\"。", Thread.CurrentThread.ManagedThreadId, Thread.GetData(storeSlot)));

                // 命名数据槽中分配的数据必须用 FreeNamedDataSlot() 释放。未命名的数据槽数据随线程的销毁而释放
                Thread.FreeNamedDataSlot("Name");
            });
            newThread.Start();
            newThread.Join();

            Console.WriteLine(String.Format("执行完新线程后，ID为{0}的线程，命名为\"Name\"的数据槽，在新线程为其设置数据 后 为\"{1}\"。", Thread.CurrentThread.ManagedThreadId, Thread.GetData(slot)));
        }

        // 不应依赖于类构造函数来初始化线程相关的静态字段[ThreadStatic]
        [ThreadStatic]
        static string name = String.Empty;
        /// <summary>
        /// 线程相关静态字段  的使用示例
        /// </summary>
        private static void TLS4StaticField()
        {
            Console.WriteLine(String.Format("ID为{0}的线程，开始为name静态字段设置数据。", Thread.CurrentThread.ManagedThreadId));
            name = "小丽";
            Console.WriteLine(String.Format("ID为{0}的线程，name静态字段数据为\"{1}\"。", Thread.CurrentThread.ManagedThreadId, name));

            Thread newThread = new Thread(() =>
            {
                Console.WriteLine(String.Format("ID为{0}的线程，为name静态字段设置数据 前 为\"{1}\"。", Thread.CurrentThread.ManagedThreadId, name));
                Console.WriteLine(String.Format("ID为{0}的线程，开始为name静态字段设置数据。", Thread.CurrentThread.ManagedThreadId));
                name = "小红";
                Console.WriteLine(String.Format("ID为{0}的线程，为name静态字段设置数据 后 为\"{1}\"。", Thread.CurrentThread.ManagedThreadId, name));
            });
            newThread.Start();
            newThread.Join();

            Console.WriteLine(String.Format("执行完新线程后，ID为{0}的线程，name静态字段数据为\"{1}\"。", Thread.CurrentThread.ManagedThreadId, name));
        }

        #endregion

        #region [ 示例：.NET下未捕获异常的处理 ]
        static object _initLock = new object();
        /// <summary>
        /// 示例：.NET下未捕获异常的处理
        /// </summary>
        public static void SampleCaptureUnhandledException()
        {
            // Do this one time for each AppDomain.
            lock (_initLock)
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            }

            throw new Exception("fafadfljlj");
        }

        static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("{0} 捕获到未处理的异常, 异常: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), e.ExceptionObject.ToString());
        }

        #endregion

        
    }
}
