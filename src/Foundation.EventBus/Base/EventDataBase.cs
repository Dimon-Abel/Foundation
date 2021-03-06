using Foundation.Core.Data;
using Foundation.EventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.EventBus.Base
{
    /// <summary>
    ///     事件源数据信息基类
    /// </summary>
    public abstract class EventDataBase : IEventData
    {
        /// <summary>
        ///     初始化一个<see cref="EventDataBase" />类型的新实例
        /// </summary>
        protected EventDataBase()
        {
            Id = XGuidHelper.GetDateGuid();
            EventTime = DateTime.Now;
        }

        /// <summary>
        ///     获取 事件编号
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     获取 事件发生时间
        /// </summary>
        public DateTime EventTime { get; }

        /// <summary>
        ///     获取或设置 触发事件的对象
        /// </summary>
        public object EventSource { get; set; }
    }
}