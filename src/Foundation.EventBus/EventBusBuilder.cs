using Foundation.EventBus.Interface;

namespace Foundation.EventBus
{
    /// <summary>
    ///     EventBus初始化
    /// </summary>
    internal class EventBusBuilder : IEventBusBuilder
    {
        private readonly IEventBus _eventBus;
        private readonly IEventHandlerTypeFinder _typeFinder;

        /// <summary>
        ///     初始化一个<see cref="EventBusBuilder" />类型的新实例
        /// </summary>
        public EventBusBuilder(IEventHandlerTypeFinder typeFinder, IEventBus eventBus)
        {
            _typeFinder = typeFinder;
            _eventBus = eventBus;
        }

        /// <summary>
        ///     初始化EventBus
        /// </summary>
        public void Build()
        {
            var types = _typeFinder.FindAll(true);
            if (types.Length == 0)
                return;
            _eventBus.SubscribeAll(types);
        }
    }
}