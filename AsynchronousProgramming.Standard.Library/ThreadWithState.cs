using System;
using System.Collections.Generic;
using System.Text;

namespace AsynchronousProgramming.Standard.Library
{
    public delegate void ThreadCallback(int lineCount);

    /// <summary>
    /// 有状态的线程
    /// </summary>
    public class ThreadWithState
    {
        string _boilerplate;
        int _value;
        ThreadCallback _threadCallback;

        public ThreadWithState(string text, int number, ThreadCallback threadCallback)
        {
            _boilerplate = text;
            _value = number;
            _threadCallback = threadCallback;
        }

        public void ThreadProc()
        {
            Console.WriteLine(_boilerplate, _value);
            // 异步执行完时调用回调
            if (_threadCallback != null)
            {
                _threadCallback(1);
            }
        }
    }
}
