using Foundation.RabbitMQ.Entity;
using Foundation.RabbitMQ.Realization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.RabbitMQ.Interface
{
    public interface IMQFactoryCenter
    {
        /// <summary>
        /// 创建连接工厂
        /// </summary>
        /// <param name="user">账号</param>
        /// <param name="password">密码</param>
        /// <param name="host">ip</param>
        /// <param name="key">工厂标识</param>
        MQConnectionFactory CreateFactory(string user, string password, string host, string key = "default");
    }
}
