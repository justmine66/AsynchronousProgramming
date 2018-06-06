using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsynchronousProgramming.Standard.Library
{
    public class TaskCompletionSourceSamples
    {
        private readonly ConcurrentDictionary<long, EchoFuture> _echoFutureDict;

        public TaskCompletionSourceSamples()
        {
            _echoFutureDict = new ConcurrentDictionary<long, EchoFuture>();
        }

        public async Task SendEchos()
        {
            for (int i = 0; i < 10; i++)
            {
                var echo = await EchoFuture(i);
                Console.WriteLine("收到第" + echo + "回声");
            }
        }

        public void ResponseEchos()
        {
            new Timer(self =>
            {
                var sequence = new Random().Next(0, 10);
                if (_echoFutureDict.TryRemove(sequence, out EchoFuture echoFuture))
                {
                    echoFuture._taskSource.TrySetResult(echoFuture.Sequence);
                }
            }).Change(1000, 500);
        }

        public Task<int> EchoFuture(int sequence)
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            _echoFutureDict.TryAdd(sequence, new EchoFuture() { Sequence = sequence, _taskSource = taskCompletionSource });

            return taskCompletionSource.Task;
        }
    }

    public class EchoFuture
    {
        public int Sequence { get; set; }
        public TaskCompletionSource<int> _taskSource;
    }
}
