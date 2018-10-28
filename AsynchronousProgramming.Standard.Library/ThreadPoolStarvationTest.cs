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

            //V1.Main();
            //V2.Main();
            //V3.Main();
            V4.Main();

            Console.ReadLine();
        }

        private class V1
        {
            public static void Main()
            {
                Task.Factory.StartNew(
                    ProducerAsync,
                    TaskCreationOptions.None);
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

        private class V2
        {
            public static void Main()
            {
                Task.Factory.StartNew(
                    ProducerAsync,
                    TaskCreationOptions.None);
            }

            static void ProducerAsync()
            {
                while (true)
                {
                    Task.Factory.StartNew(
                    Process,
                    TaskCreationOptions.None);

                    Thread.Sleep(200);
                }
            }

            static async Task Process()
            {
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

        private class V3
        {
            public static void Main()
            {
                Task.Factory.StartNew(
                    ProducerAsync,
                    TaskCreationOptions.LongRunning);
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

        private class V4
        {
            public static void Main()
            {
                Task.Factory.StartNew(
                    ProducerAsync,
                    TaskCreationOptions.None);
            }

            static void ProducerAsync()
            {
                while (true)
                {
                    Task.Factory.StartNew(
                    Process,
                    TaskCreationOptions.PreferFairness);

                    Thread.Sleep(200);
                }
            }

            static async Task Process()
            {
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
}
