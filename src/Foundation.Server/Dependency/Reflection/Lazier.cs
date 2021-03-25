using Microsoft.Extensions.DependencyInjection;
using System;

namespace Foundation.Server.Dependency.Reflection
{
    /// <summary>
    ///     Lazy延迟加载解析器
    /// </summary>
    internal class Lazier<T> : Lazy<T> where T : class
    {
        /// <summary>
        ///     初始化一个<see cref="Lazier{T}" />类型的新实例
        /// </summary>
        public Lazier(IServiceProvider provider)
            : base(provider.GetRequiredService<T>)
        {
        }
    }
}
