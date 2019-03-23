using System;
using System.Threading;

namespace AsynchronousProgramming.Standard.Library
{
    public class Synchronizer<TShared, TIRead, TIWrite>
        where TShared : TIWrite, TIRead
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private TShared _shared;

        public Synchronizer(TShared shared)
        {
            _shared = shared;
        }

        public void Read(Action<TIRead> functor)
        {
            _lock.EnterReadLock();
            try
            {
                functor(_shared);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Write(Action<TIWrite> functor)
        {
            _lock.EnterWriteLock();
            try
            {
                functor(_shared);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }

    interface IReadFromShared
    {
        string GetValue();
    }

    interface IWriteToShared
    {
        void SetValue(string value);
    }

    class MySharedClass : IReadFromShared, IWriteToShared
    {
        string _foo;

        public string GetValue()
        {
            return _foo;
        }

        public void SetValue(string value)
        {
            _foo = value;
        }
    }

    class ReadWriteDemo
    {
        void Foo(Synchronizer<MySharedClass, IReadFromShared, IWriteToShared> sync)
        {
            sync.Write(share => share.SetValue("new value"));
            sync.Read(share =>
            {
                var value = share.GetValue();
                Console.WriteLine(value);
            });
        }
    }
}
