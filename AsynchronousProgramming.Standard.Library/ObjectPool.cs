using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AsynchronousProgramming.Standard.Library
{
    /// <summary>
    /// 缓冲对象代理接口
    /// </summary>
    public interface ICacheObjectProxy<out T> : IDisposable
    {
        /// <summary>
        /// 创建实际对象
        /// </summary>
        void Create(params Object[] paramsArr);

        /// <summary>
        /// 重置实际对象
        /// </summary>
        void Reset();

        /// <summary>
        /// 返回实际对象
        /// </summary>
        T GetInnerObject();

        /// <summary>
        /// 判断用户实际对象的有效性，是对象池决定是否重新创建实际对象的标志
        /// </summary>
        bool IsValidate();
    }

    /// <summary>
    /// 对象池中项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PoolItem<T> where T : ICacheObjectProxy<T>
    {
        private ICacheObjectProxy<T> _innerObjectProxy;
        private Object[] _createParams;
        private bool _using;

        public PoolItem(Object[] paramsArr)
        {
            _createParams = paramsArr;
            this.Create();
        }

        /// <summary>
        /// 创建内部实际对象
        /// </summary>
        private void Create()
        {
            _using = false;
            _innerObjectProxy = System.Activator.CreateInstance(typeof(T)) as ICacheObjectProxy<T>;
            _innerObjectProxy.Create(_createParams);
        }

        /// <summary>
        /// 重置内部实际对象
        /// </summary>
        public void Reset()
        {
            _using = false;
            _innerObjectProxy.Reset();
        }

        /// <summary>
        /// 释放内部实际对象
        /// </summary>
        public void Dispose()
        {
            _innerObjectProxy.Dispose();
        }

        /// <summary>
        /// 获取内部存储的实际对象
        /// </summary>
        public T InnerObject
        {
            get { return _innerObjectProxy.GetInnerObject(); }
        }

        /// <summary>
        /// 内部存储的实际对象的 HashCode 标识
        /// </summary>
        public int InnerObjectHashcode
        {
            get { return InnerObject.GetHashCode(); }
        }

        /// <summary>
        /// 判断用户实际对象的有效性，是对象池决定是否重置实际对象的标志
        /// </summary>
        public bool IsValidate
        {
            get { return _innerObjectProxy.IsValidate(); }
        }

        /// <summary>
        /// 是否正在使用
        /// </summary>
        public bool Using
        {
            get { return _using; }
            set { _using = value; }
        }
    }

    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ObjectPool<T> where T : ICacheObjectProxy<T>
    {
        // 最大容量
        private int _maxPoolCount = 30;
        // 最小容量
        private int _minPoolCount = 5;
        // 已存容量
        private int _currentCount;
        // 空闲+被用 对象列表
        private Hashtable _listObjects;
        // 空闲对象索引列表
        private List<int> _listFreeIndex;
        // 被用对象索引列表
        private List<int> _listUsingIndex;

        private object[] _objCreateParams;

        // 最大空闲时间
        private int _maxIdleTime = 120;

        // 定时清理对象池对象
        private Timer _timer = null;

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="maxPoolCount">最小容量</param>
        /// <param name="minPoolCount">最大容量</param>
        /// <param name="create_params">待创建的实际对象的参数</param>
        public ObjectPool(int maxPoolCount, int minPoolCount, object[] create_params)
        {
            if (minPoolCount < 0 || maxPoolCount < 1 || minPoolCount > maxPoolCount)
            {
                throw (new Exception("Invalid parameter!"));
            }

            _maxPoolCount = maxPoolCount;
            _minPoolCount = minPoolCount;
            _objCreateParams = create_params;

            _listObjects = new Hashtable(_maxPoolCount);
            _listFreeIndex = new List<int>(_maxPoolCount);
            _listUsingIndex = new List<int>(_maxPoolCount);

            for (int i = 0; i < _minPoolCount; i++)
            {
                var pitem = new PoolItem<T>(_objCreateParams);
                _listObjects.Add(pitem.InnerObjectHashcode, pitem);
                _listFreeIndex.Add(pitem.InnerObjectHashcode);
            }

            _currentCount = _listObjects.Count;

            _timer = new Timer(new TimerCallback(AutoReleaseObject),
                              null, _maxIdleTime * 1000, _maxIdleTime * 1000);
        }

        /// <summary>
        /// 释放该对象池
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                foreach (DictionaryEntry de in _listObjects)
                {
                    ((PoolItem<T>)de.Value).Dispose();
                }
                _listObjects.Clear();
                _listFreeIndex.Clear();
                _listUsingIndex.Clear();
            }
        }

        /// <summary>
        /// 已存容量
        /// </summary>
        public int CurrentCount
        {
            get { return _currentCount; }
        }

        /// <summary>
        /// 当前已被用的对象数
        /// </summary>
        public int UsingCount
        {
            get { return _listUsingIndex.Count; }
        }

        /// <summary>
        /// 获取一个对象实例
        /// </summary>
        /// <returns>返回内部实际对象，若返回null则线程池已满</returns>
        public T GetOne()
        {
            lock (this)
            {
                // 如果没有空闲对象，且已存容量<总容量， 则创建新对象
                if (_listFreeIndex.Count == 0)
                {
                    if (_currentCount == _maxPoolCount)
                        return default(T);

                    var pNewItem = new PoolItem<T>(_objCreateParams);
                    _listObjects.Add(pNewItem.InnerObjectHashcode, pNewItem);
                    _listFreeIndex.Add(pNewItem.InnerObjectHashcode);
                    _currentCount++;
                }

                int nFreeIndex = _listFreeIndex[0];
                var pItem = _listObjects[nFreeIndex] as PoolItem<T>;
                _listFreeIndex.RemoveAt(0);
                _listUsingIndex.Add(nFreeIndex);

                if (!pItem.IsValidate)
                {
                    pItem.Reset();
                }

                pItem.Using = true;
                return pItem.InnerObject;
            }
        }

        /// <summary>
        /// 将对象池中指定的对象重置并设置为空闲状态
        /// </summary>
        public void ReturnOne(T obj)
        {
            lock (this)
            {
                int key = obj.GetHashCode();
                if (_listUsingIndex.Contains(key) && _listObjects.ContainsKey(key))
                {
                    var item = _listObjects[key] as PoolItem<T>;
                    item.Using = false;
                    item.Reset();
                    _listUsingIndex.Remove(key);
                    _listFreeIndex.Add(key);
                }
            }
        }

        /// <summary>
        /// 手动清理对象池
        /// </summary>
        public void ManualReleaseObject()
        {
            AutoReleaseObject(null);
        }

        /// <summary>
        /// 自动清理对象池（对大于 最小容量 的空闲对象进行释放）
        /// </summary>
        private void AutoReleaseObject(Object obj)
        {
            if (_listFreeIndex.Count > _minPoolCount)
            {
                DecreaseObject(_listFreeIndex.Count - _minPoolCount);
            }
        }

        /// <summary>
        /// 减少对象池中指定数量的空闲对象（若此长度>当前空闲对象数，则为释放对象池中所有空闲对象）
        /// </summary>
        /// <param name="decreaseCount">指定数量的空闲对象</param>
        /// <returns>返回实际减少的空闲对象数量（实际释放多少对象数跟“最小容量+当前对象数”有关）</returns>
        public int DecreaseObject(int decreaseCount)
        {
            int nDecrease = decreaseCount;

            lock (this)
            {
                if (nDecrease < 0)
                {
                    return 0;
                }

                if (nDecrease > _listFreeIndex.Count)
                {
                    nDecrease = _listFreeIndex.Count;
                }

                for (int i = 0; i < nDecrease; i++)
                {
                    _listObjects.Remove(_listFreeIndex[i]);
                }

                _listFreeIndex.Clear();
                _listUsingIndex.Clear();

                foreach (DictionaryEntry de in _listObjects)
                {
                    var pitem = de.Value as PoolItem<T>;
                    if (pitem.Using)
                    {
                        _listUsingIndex.Add(pitem.InnerObjectHashcode);
                    }
                    else
                    {
                        _listFreeIndex.Add(pitem.InnerObjectHashcode);
                    }
                }

                _currentCount -= nDecrease;
                return nDecrease;
            }
        }
    }
}
