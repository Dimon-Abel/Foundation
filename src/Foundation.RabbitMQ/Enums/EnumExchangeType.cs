using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Foundation.RabbitMQ.Enums
{
    /// <summary>
    /// 交换机模式
    /// </summary>
    public enum EnumExchangeType
    {
        [Description("Direct Exchange")]
        Direct,
        [Description("Fanout Exchange")]
        Fanout,
        [Description("Headers Exchange")]
        Headers,
        [Description("Topic Exchange")]
        Topic
    }
}
