namespace Foundation.EventBus.Interface
{
    /// <summary>
    ///     定义获取<see cref="IEventHandler" />实例的方式
    /// </summary>
    public interface IEventHandlerFactory
    {
        /// <summary>
        ///     获取事件处理器实例
        /// </summary>
        /// <returns></returns>
        IEventHandler GetHandler();

        /// <summary>
        ///     释放事件处理器实例
        /// </summary>
        /// <param name="handler"></param>
        void ReleaseHandler(IEventHandler handler);
    }
}