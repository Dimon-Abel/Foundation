using Foundation.Server.Domain.Interface;
using Foundation.Server.Domain.Reflection;
using Foundation.Server.Host.Interface;
using Foundation.Server.Host.Reflection;
using Foundation.Server.Packs;
using Foundation.Server.Packs.Enum;
using Foundation.Server.Reflection.Interface;
using Foundation.Server.Reflection.Realization;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Foundation.Server.BasePacks
{
    /// <summary>
    ///     领域模块
    /// </summary>
    [Description("领域模块")]
    public class CorePack : PackBase
    {
        /// <summary>
        ///     获取 模块级别
        /// </summary>
        public override PackLevel Level => PackLevel.Core;

        /// <summary>
        ///     将模块服务添加到依赖注入服务容器中
        /// </summary>
        /// <param name="services">依赖注入服务容器</param>
        /// <returns></returns>
        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IAllAssemblyFinder, AppDomainAllAssemblyFinder>();
            services.AddSingleton<IEntityTypeFinder, EntityTypeFinder>();
            services.AddSingleton<IInputDtoTypeFinder, InputDtoTypeFinder>();
            services.AddSingleton<IOutputDtoTypeFinder, OutputDtoTypeFinder>();
            // 自定义的终结点路由器
            services.AddTransient<IEndpointRouter, EndpointRouter>();

            return services;
        }
    }
}