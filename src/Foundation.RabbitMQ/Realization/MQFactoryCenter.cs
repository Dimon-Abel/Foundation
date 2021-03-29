using Foundation.RabbitMQ.Entity;
using Foundation.RabbitMQ.Interface;
using Foundation.Server.Dependency;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace Foundation.RabbitMQ.Realization
{
    public class MQFactoryCenter : IMQFactoryCenter
    {
        private static ConcurrentDictionary<string, MQConnectionFactory> _factories = new ConcurrentDictionary<string, MQConnectionFactory>();
        static MQFactoryCenter() { }

        /// <summary>
        /// 创建连接工厂
        /// </summary>
        /// <param name="user">账号</param>
        /// <param name="password">密码</param>
        /// <param name="host">Host</param>
        /// <param name="key">工厂名称</param>
        public MQConnectionFactory CreateFactory(string user, string password, string host, string factoryName = "default")
        {
            if (_factories.TryGetValue(factoryName, out var factory))
                return factory;

            var newFactory = new MQConnectionFactory(user, password, host);
            _factories.AddOrUpdate(factoryName, newFactory, (key, factory) =>
            {
                factory.UpdateUserName(user).UpdatePassword(password).UpdateHost(host);
                return factory;
            });
            return newFactory;
        }
    }
}
