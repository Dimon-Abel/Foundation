using Microsoft.AspNetCore.Http;
using System;

namespace Foundation.Server.Host.Reflection
{
    /// <summary>
    /// 终结点
    /// </summary>
    public class Endpoint
    {
        public Endpoint(string name, string path, Type handlerType)
        {
            Name = name;
            Path = path;
            Handler = handlerType;
        }

        /// <summary>
        /// 终结点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 终结点路径
        /// </summary>
        public PathString Path { get; set; }

        /// <summary>
        /// 终结点处理程序
        /// </summary>
        public Type Handler { get; set; }
    }
}
