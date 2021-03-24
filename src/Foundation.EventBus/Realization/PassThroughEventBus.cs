using Foundation.EventBus.Base;
using Foundation.EventBus.Interface;
using Microsoft.Extensions.Logging;

namespace Foundation.EventBus.Realization
{
    /// <summary>
    ///     一个事件总线，当有消息被派发到消息总线时，消息总线将不做任何处理与路由，而是直接将消息推送到订阅方
    /// </summary>
    internal class PassThroughEventBus : EventBusBase
    {
        /// <summary>
        ///     初始化一个<see cref="PassThroughEventBus" />类型的新实例
        /// </summary>
        public PassThroughEventBus(IEventStore eventStore, ILoggerFactory loggerFactory)
            : base(eventStore, loggerFactory)
        {
        }
    }
}