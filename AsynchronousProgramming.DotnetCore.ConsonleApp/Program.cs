using AsynchronousProgramming.Standard.Library;
using System;

namespace AsynchronousProgramming.DotnetCore.ConsonleApp
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
            // 示例：ExecutionContext
            //Example_ExecutionContext();
            // 示例：DoubleCheckLock
            //SampledMain.DoubleCheckLock_Test();
            //MethodImpl_Text.Test();
            //示例：线程池线程协作取消
            //ThreadPool_Cancel.ThreadPool_Cancel_test();
            //示例: ManualResetEventSlim
            //ManualResetEventSlim_Test.Test();
            //示例: CountdownEvent
            //CountdownEvent_Test.Test();
            //示例：Barrier
            Barrier_Test.Test();
        }
    }
}
