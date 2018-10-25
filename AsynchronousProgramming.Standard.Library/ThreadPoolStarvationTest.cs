using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsynchronousProgramming.Standard.Library
{
    public class ThreadPoolStarvationTest
    {
        public static void Test()
        {
            Console.WriteLine(Environment.ProcessorCount);

            ThreadPool.SetMinThreads(8, 8);

            Task.Factory.StartNew(
                ProducerAsync,
                TaskCreationOptions.PreferFairness);

            Console.ReadLine();
        }

        static void ProducerAsync()
        {
            while (true)
            {
                Process();

                Thread.Sleep(200);
            }
        }

        static async Task Process()
        {
            await Task.Yield();

            var tcs = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                tcs.SetResult(true);
            });

            tcs.Task.Wait();

            Console.WriteLine("Ended - " + DateTime.Now.ToLongTimeString());
        }
    }
}
