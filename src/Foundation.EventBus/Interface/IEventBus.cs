namespace Foundation.EventBus.Interface
{
    /// <summary>
    ///     定义线程总线
    /// </summary>
    public interface IEventBus : IEventSubscriber, IEventPublisher
    {
    }
}
