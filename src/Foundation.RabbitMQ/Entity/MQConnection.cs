using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Foundation.RabbitMQ.Entity
{
    public class MQConnection : IDisposable
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        private static IConnection _connection { get; set; }

        /// <summary>
        /// 通道集合
        /// </summary>
        private static ConcurrentDictionary<string, MQModel> _models = new ConcurrentDictionary<string, MQModel>();

        public MQConnection(IConnection connection) => _connection = connection;

        /// <summary>
        /// 创建通道
        /// </summary>
        /// <param name="key">通道标识</param>
        public MQModel CreateOrGetModel(string key = "default")
        {
            if (_models.TryGetValue(key, out var model))
                return model;

            var newModel = new MQModel(_connection.CreateModel());
            _models.AddOrUpdate(key, newModel, (key, model) => model);
            return newModel;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
