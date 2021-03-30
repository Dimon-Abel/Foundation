namespace Foundation.RabbitMQ.Options
{
    /// <summary>
    /// 消息生产者配置
    /// </summary>
    public class ProducerOptions
    {
        /// <summary>
        /// 消息批量发送的最大数量
        /// </summary>
        public int MaxPublishMessages { get; } = 50;
    }
}
