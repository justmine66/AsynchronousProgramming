using System;
using System.Collections.Generic;
using System.Text;

namespace AsynchronousProgramming.Standard.Library
{
    using System.Threading;

    /// <summary>
    /// 示例 CountdownEvent
    /// </summary>
    public class CountdownEvent_Test
    {
        public static void Test()
        {
            Console.WriteLine("初始化CountdownEvent计数为1000");
            var cde = new CountdownEvent(1000);
            // CurrentCount（当前值）1200允许大于InitialCount（初始值）1000
            cde.AddCount(200);
            Console.WriteLine("增加CountdownEvent计数200至1200");
            var thread = new Thread(o =>
            {
                int i = 1200;
                for (int j = 1; j <= i; j++)
                {
                    if (j == i)
                        Console.WriteLine("CurrentCount为1200，所以必须调用Signal()1200次");
                    bool isS = cde.Signal();
                    if (isS)
                    {
                        Console.WriteLine("CurrentCount为{0}", j);
                    }
                }
            });
            thread.Start();
            Console.WriteLine("调用CountdownEvent的Wait()方法");
            cde.Wait();
            Console.WriteLine("CountdownEvent计数为0，完成等待");
            cde.Dispose();

            Console.Read();
        }
    }
}
