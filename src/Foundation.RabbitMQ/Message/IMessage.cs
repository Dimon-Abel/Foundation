using Foundation.RabbitMQ.Event;

namespace Foundation.RabbitMQ.Message
{
    /// <summary>
    /// 消息
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 消息Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 事件对象
        /// </summary>
        public byte[] Event { get; }

        /// <summary>
        /// 事件元数据
        /// </summary>
        public EventMetadata EventMetadata { get; }

        /// <summary>
        /// 获取消息的字节数组
        /// </summary>
        /// <returns> </returns>
        byte[] GetBytes();
    }
}