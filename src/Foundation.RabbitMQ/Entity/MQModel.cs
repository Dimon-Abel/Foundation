using Foundation.RabbitMQ.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Foundation.RabbitMQ.Entity
{
    public class MQModel : IDisposable
    {
        /// <summary>
        /// 通道对象
        /// </summary>
        private static IModel _model { get; set; }
        /// <summary>
        /// 启用消息确认
        /// </summary>
        private bool _enableConfirm { get; set; }
        public MQModel(IModel model, bool enableConfirm)
        {
            _model = model;
            _enableConfirm = enableConfirm;
        }
        /// <summary>
        /// 创建交换机
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="type">交换机类型</param>
        /// <param name="durable">是否持久化信息</param>
        /// <param name="autoDelete">是否自动删除,自动删除的前提是：致少有一个消费者连接到这个队列，之后所有与这个队列连接的消费者都断开 时，才会自动删除。</param>
        /// <param name="args">其它参数</param>
        /// <returns></returns>
        public MQModel CreateExChange(string exchangeName, EnumExchangeType type, bool durable = false, bool autoDelete = false, IDictionary<string, object> args = null)
        {
            _model.ExchangeDeclare(exchangeName, type.ToString().ToLower(), durable, autoDelete, args);
            return this;
        }
        /// <summary>
        /// 创建队列
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="durable">是否持久化信息</param>
        /// <param name="exclusive">是否排他, 如果一个队列声明为排他队列，该队列仅对首次声明它的连接可见，并在连接断开时自动删除。</param>
        /// <param name="autoDelete">是否自动删除,自动删除的前提是：致少有一个消费者连接到这个队列，之后所有与这个队列连接的消费者都断开 时，才会自动删除。</param>
        /// <param name="args">其它参数</param>
        /// <returns></returns>
        public MQModel CreateOrGetQueue(string queueName, bool durable = false, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> args = null)
        {
            _model.QueueDeclare(queueName, durable, exclusive, autoDelete);
            return this;
        }

        /// <summary>
        /// 交换机绑定队列
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routeKey"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public MQModel QueueBind(string queueName, string exchangeName, string routeKey, IDictionary<string, object> args = null)
        {
            _model.QueueBind(queueName, exchangeName, routeKey, args);
            return this;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routeKey">routeKey</param>
        /// <param name="properties"></param>
        /// <param name="body"></param>
        public void Publish(string exchangeName, string routeKey, IBasicProperties properties, ReadOnlyMemory<byte> body) =>
            _model.BasicPublish(exchangeName, routeKey, properties, body);

        /// <summary>
        /// 创建消费者
        /// </summary>
        /// <param name="action">回调委托</param>
        public EventingBasicConsumer CreateConsumer(Func<object, BasicDeliverEventArgs, bool> action)
        {
            EventingBasicConsumer consumer = new EventingBasicConsumer(_model);
            consumer.Received += (obj, args) =>
            {
                var state = action.Invoke(obj, args);
                // 确认消息被消费
                if (state) _model.BasicAck(args.DeliveryTag, false);
            };
            return consumer;
        }

        public void Dispose() => _model.Dispose();
    }
}
