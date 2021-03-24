using Foundation.Server.Packs;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Foundation.Server.ServerBuilder.Interface
{
    /// <summary>
    ///     定义框架服务构建器
    /// </summary>
    public interface IServerBuilder
    {
        /// <summary>
        /// 服务集合
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// 获取功能包集合
        /// </summary>
        List<PackBase> Packs { get; }

        /// <summary>
        /// 服务框架构建入口
        /// </summary>
        /// <returns></returns>
        IServerBuilder Build();
    }
}
