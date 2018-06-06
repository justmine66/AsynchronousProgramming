using AsynchronousProgramming.Standard.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace AsynchronousProgramming.Dotnet.ConsonleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // 示例：使用帮助器类传递参数并使用回调函数检索数据
            //SampledMain.SimpleCallBackWithParam();
            // 示例：使用Abort终止线程
            //SampledMain.SimpleAbort();
            // 示例：托管TSL（数据槽|线程相关静态字段）中数据的唯一性
            //SampledMain.SimpleTLS();
            //示例：.NET下未捕获异常的处理
            //SampledMain.SampleCaptureUnhandledException();
            // 示例：ThreadPool.RegisterWaitForSingleObject 
            //SampledMain.Example_RegisterWaitForSingleObject();
            // 示例：ExecutionContext
            // 示例：DoubleCheckLock
            //SampledMain.DoubleCheckLock_Test();
            //Synchronization_Test.Test();
            MethodImpl_Text.Test();
        }

        #region [ 示例：ExecutionContext  ]

        /// <summary>
        /// 示例：ExecutionContext 
        /// 1)	在线程间共享逻辑调用上下文数据（CallContext）。
        /// 2)	为了提升性能，取消\恢复执行上下文的流动。
        /// 3)	在当前线程上的指定执行上下文中运行某个方法。
        /// </summary>
        public static void Example_ExecutionContext()
        {
            CallContext.LogicalSetData("Name", "小红");
            Console.WriteLine("主线程中Name为：{0}", CallContext.LogicalGetData("Name"));

            // 1)	在线程间共享逻辑调用上下文数据（CallContext）。
            ThreadPool.QueueUserWorkItem((state) =>
            {
                Console.WriteLine("ThreadPool中Name为：{0}", CallContext.LogicalGetData("Name"));
            });
            Thread.Sleep(500);
            Console.WriteLine();
            // 2)	为了提升性能，取消\恢复执行上下文的流动。
            ThreadPool.UnsafeQueueUserWorkItem((state) =>
            {
                Console.WriteLine("ThreadPool线程使用Unsafe异步执行方法来取消执行上下文的流动。Name为：\"{0}\""
                , CallContext.LogicalGetData("Name"));
            }, null);
            Console.WriteLine("2)为了提升性能，取消/恢复执行上下文的流动。");
            AsyncFlowControl asyncFlowControl = ExecutionContext.SuppressFlow();
            ThreadPool.QueueUserWorkItem((Object obj)
                => Console.WriteLine("(取消ExecutionContext流动)ThreadPool线程中Name为：\"{0}\"", CallContext.LogicalGetData("Name")));
            Thread.Sleep(500);
            // 恢复不推荐使用ExecutionContext.RestoreFlow()
            asyncFlowControl.Undo();
            ThreadPool.QueueUserWorkItem((Object obj)
               => Console.WriteLine("(恢复ExecutionContext流动)ThreadPool线程中Name为：\"{0}\"", CallContext.LogicalGetData("Name")));
            Thread.Sleep(500);
            Console.WriteLine();
            // 3)	在当前线程上的指定执行上下文中运行某个方法。(通过获取调用上下文数据验证)
            Console.WriteLine("3)在当前线程上的指定执行上下文中运行某个方法。(通过获取调用上下文数据验证)");
            ExecutionContext executionContext = ExecutionContext.Capture();
            // 若使用 Thread.CurrentThread.ExecutionContext 会报“无法应用以下上下文: 跨 AppDomains 封送的上下文、不是通过捕获操作获取的上下文或已作为 Set 调用的参数的上下文。”
            // 若使用Thread.CurrentThread.ExecutionContext.CreateCopy()会报“只能复制新近捕获(ExecutionContext.Capture())的上下文”。 
            ExecutionContext.SuppressFlow();
            ThreadPool.QueueUserWorkItem((Object obj) =>
                {
                    ExecutionContext innerExecutionContext = obj as ExecutionContext;
                    ExecutionContext.Run(innerExecutionContext, (Object state)
                        => Console.WriteLine("ThreadPool线程中Name为：\"{0}\"", CallContext.LogicalGetData("Name")), null);
                }, executionContext);

            Console.Read();
        }

        #endregion
    }
}
