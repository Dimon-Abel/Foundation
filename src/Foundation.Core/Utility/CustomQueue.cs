using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Foundation.Core.Utility
{
    /// <summary>
    /// 自定义线程安全队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomQueue<T>
    {
        private static ConcurrentQueue<T> _queue;
        private static readonly Lazy<CustomQueue<T>> _instance = new Lazy<CustomQueue<T>>(() => new CustomQueue<T>());
        private CustomQueue()
        {
            if (_queue == null)
                _queue = new ConcurrentQueue<T>();
        }
        /// <summary>
        /// 获取队列对象
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static CustomQueue<T> Instance() => _instance.Value;
        /// <summary>
        /// 添加队列元素
        /// </summary>
        /// <param name="entity"></param>
        public void Enqueue(T entity) => _queue.Enqueue(entity);
        /// <summary>
        /// 获取队列元素
        /// </summary>
        /// <returns></returns>
        public T TryDequeue()
        {
            if (_queue.TryDequeue(out T entity))
                return entity;
            return default(T);
        }
    }
}
