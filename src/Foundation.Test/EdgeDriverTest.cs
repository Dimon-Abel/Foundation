using Foundation.RabbitMQ.Realization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xunit.Sdk;
using Foundation.Core.Extensions;

namespace Foundation.Test
{
    [TestClass]
    public class EdgeDriverTest
    {
        [TestMethod]
        public void MQSendMessage()
        {
            Console.WriteLine("MQSendMessage start");
            MQFactoryCenter mQFactoryCenter = new MQFactoryCenter();
            var user = "user"; var pwd = "password"; var host = "2.59.151.122";

            var channel = mQFactoryCenter.CreateFactory(user, pwd, host).CreateConnection().CreateOrGetModel("default");
            #region Direct

            var directExchangeName = "directExchange";
            var directQueueName = "directQueue";
            var routeKey = "test";

            channel.CreateExChange(directExchangeName, RabbitMQ.Enums.EnumExchangeType.Direct);
            channel.CreateOrGetQueue(directQueueName);
            channel.QueueBind(directQueueName, directExchangeName, routeKey);
            channel.Publish(directExchangeName, routeKey, null, "新的一条信息".ToBytes());

            #endregion
        }
    }
}
