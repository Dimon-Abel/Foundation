using Foundation.Core.Extensions;
using Foundation.Server.Dependency;
using Foundation.Server.Packs.Enum;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Foundation.Server.Packs
{
    public abstract class PackBase
    {
        /// <summary>
        ///     获取 模块级别，级别越小越先启动
        /// </summary>
        public virtual PackLevel Level => PackLevel.Business;

        /// <summary>
        ///     获取 模块启动顺序，模块启动的顺序先按级别启动，同一级别内部再按此顺序启动，
        ///     级别默认为0，表示无依赖，需要在同级别有依赖顺序的时候，再重写为>0的顺序值
        /// </summary>
        public virtual int Order => 0;

        /// <summary>
        ///     获取 是否已可用
        /// </summary>
        public bool IsEnabled { get; protected set; }

        /// <summary>
        ///     将模块服务添加到依赖注入服务容器中
        /// </summary>
        /// <param name="services">依赖注入服务容器</param>
        /// <returns></returns>
        public abstract IServiceCollection AddServices(IServiceCollection services);

        /// <summary>
        ///     应用模块服务
        /// </summary>
        /// <param name="provider">服务提供者</param>
        public virtual void UsePack(IServiceProvider provider) => IsEnabled = true;

        public virtual void UsePack(IApplicationBuilder app) => UsePack(app.ApplicationServices);

        /// <summary>
        ///     获取当前模块的依赖模块类型
        /// </summary>
        /// <returns></returns>
        internal Type[] GetDependModuleTypes()
        {
            var depends = GetType().GetAttribute<DependsOnPacksAttribute>();
            return depends == null ? new Type[0] : depends.DependedModuleTypes;
        }
    }
}