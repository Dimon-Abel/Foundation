using Foundation.EventBus.Interface;
using System;

namespace Foundation.EventBus.Realization
{
    /// <summary>
    ///     即时生命周期的事件处理器实例获取方式
    /// </summary>
    internal class TransientEventHandlerFactory<TEventHandler> : IEventHandlerFactory
        where TEventHandler : IEventHandler, new()
    {
        /// <summary>
        ///     获取事件处理器实例
        /// </summary>
        /// <returns></returns>
        public IEventHandler GetHandler()
        {
            return new TEventHandler();
        }

        /// <summary>
        ///     释放事件处理器实例
        /// </summary>
        /// <param name="handler"></param>
        public void ReleaseHandler(IEventHandler handler)
        {
            (handler as IDisposable)?.Dispose();
        }
    }
}