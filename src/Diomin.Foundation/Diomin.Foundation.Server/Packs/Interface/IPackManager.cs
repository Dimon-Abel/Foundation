using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Foundation.Server.Packs.Interface
{
    public interface IPackManager
    {
        /// <summary>
        ///     获取 自动检索到的所有模块信息
        /// </summary>
        IEnumerable<PackBase> SourcePacks { get; }

        /// <summary>
        ///     获取 最终加载的模块信息集合
        /// </summary>
        IEnumerable<PackBase> LoadedPacks { get; }

        /// <summary>
        ///     加载模块服务
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <returns>服务容器</returns>
        IServiceCollection LoadPacks(IServiceCollection services);

        /// <summary>
        ///     应用模块服务
        /// </summary>
        /// <param name="provider">服务提供者</param>
        void UsePack(IServiceProvider provider);
    }
}
