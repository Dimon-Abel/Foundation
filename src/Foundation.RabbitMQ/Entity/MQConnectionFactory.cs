using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.RabbitMQ.Entity
{
    public class MQConnectionFactory
    {
        /// <summary>
        /// 连接工厂对象
        /// </summary>
        private static ConnectionFactory _factory { get; set; }
        /// <summary>
        /// 连接对象
        /// </summary>
        private static MQConnection _connection { get; set; }

        #region 初始化

        /// <summary>
        /// 获取连接工厂对象
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <param name="host">Host</param>
        public MQConnectionFactory(string userName = null, string password = null, string host = null)
        {
            if (_factory == null)
                _factory = new ConnectionFactory();
            _factory.UserName = userName;
            _factory.Password = password;
            _factory.HostName = host;
        }

        /// <summary>
        /// 更新账号
        /// </summary>
        /// <param name="userName">连接账号</param>
        public MQConnectionFactory UpdateUserName(string userName)
        {
            _factory.UserName = userName;
            return this;
        }
        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="password">密码</param>
        public MQConnectionFactory UpdatePassword(string password)
        {
            _factory.Password = password;
            return this;
        }
        /// <summary>
        /// 更新ip
        /// </summary>
        /// <param name="host">ip</param>
        public MQConnectionFactory UpdateHost(string host)
        {
            _factory.HostName = host;
            return this;
        }

        #endregion

        /// <summary>
        /// 创建连接
        /// </summary>
        public MQConnection CreateConnection()
        {
            if (_connection == null)
                _connection = new MQConnection(_factory.CreateConnection());
            return _connection;
        }
    }
}
