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
        [Description("直接交换器，工作方式类似于单播，Exchange会将消息发送完全匹配ROUTING_KEY的Queue")]
        Direct,
        [Description("广播是式交换器，不管消息的ROUTING_KEY设置为什么，Exchange都会将消息转发给所有绑定的Queue。")]
        Fanout,
        [Description("消息体的header匹配（ignore）")]
        Headers,
        [Description(@"主题交换器，工作方式类似于组播，Exchange会将消息转发和ROUTING_KEY匹配模式相同的所有队列，
比如，ROUTING_KEY为user.stock的Message会转发给绑定匹配模式为 * .stock,user.stock， * . * 和#.user.stock.#的队列。（ * 表是匹配一个任意词组，#表示匹配0个或多个词组）")]
        Topic
    }
}
