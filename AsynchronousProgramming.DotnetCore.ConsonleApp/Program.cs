using AsynchronousProgramming.Standard.Library;
using System;
using System.Threading.Tasks;

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
            // 示例：.NET下未捕获异常的处理
            //SampledMain.SampleCaptureUnhandledException();
            //MainAsync().GetAwaiter().GetResult();
            //示例：线程池饥饿
            ThreadPoolStarvationTest.Test();

            Console.Read();
        }
        static async Task MainAsync()
        {
            //示例：TaskCompletionSource
            var tcs = new TaskCompletionSourceSamples();
            tcs.ResponseEchos();
            await tcs.SendEchos();
        }
    }
}
