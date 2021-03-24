using Foundation.EventBus.Interface;

namespace Foundation.EventBus.Realization
{
    /// <summary>
    ///     单例生命周期的事件处理器实例获取方式
    /// </summary>
    internal class SingletonEventHandlerFactory : IEventHandlerFactory
    {
        /// <summary>
        ///     初始化一个<see cref="SingletonEventHandlerFactory" />类型的新实例
        /// </summary>
        public SingletonEventHandlerFactory(IEventHandler handler)
        {
            HandlerInstance = handler;
        }

        public IEventHandler HandlerInstance { get; }

        /// <summary>
        ///     获取事件处理器实例
        /// </summary>
        /// <returns></returns>
        public IEventHandler GetHandler()
        {
            return HandlerInstance;
        }

        /// <summary>
        ///     释放事件处理器实例
        /// </summary>
        /// <param name="handler"></param>
        public void ReleaseHandler(IEventHandler handler)
        {
        }
    }
}