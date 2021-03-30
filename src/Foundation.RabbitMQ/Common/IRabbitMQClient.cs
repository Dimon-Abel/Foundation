namespace Foundation.RabbitMQ.Common
{
    /// <summary>
    /// RabbitMQ客户端
    /// </summary>
    public interface IRabbitMQClient
    {
        /// <summary>
        /// 获取通道对象
        /// </summary>
        /// <returns></returns>
        public ChannelObject GetChannel();
    }
}
