using Foundation.Core.Data;
using Foundation.Server.Middlewate;
using Foundation.Server.Reflection.Interface;
using Foundation.Server.Reflection.Realization;
using Foundation.Server.ServerBuilder.Interface;
using Foundation.Server.ServerBuilder.Realization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Foundation.Server
{
    /// <summary>
    ///     依赖注入服务集合扩展
    /// </summary>
    public static class ServerExtensions
    {
        /// <summary>
        /// 添加服务框架
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServerBuilder AddFoundationServer(this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));
            //初始化所有程序集查找器，如需更改程序集查找逻辑，请事先赋予自定义查找器的实例
            if (Singleton<IAllAssemblyFinder>.Instance == null)
                Singleton<IAllAssemblyFinder>.Instance = new AppDomainAllAssemblyFinder();

            IServerBuilder builder = new DefaultServerBuilder(services).Build();
            services.AddSingleton(builder);
            return builder;
        }

        /// <summary>
        /// 非Web启动时应用服务框架入口
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseFoundationServer(this IApplicationBuilder app)
        {
            var provider = app.ApplicationServices;
            var builder = provider.GetService<IServerBuilder>();
            builder.Packs.ForEach(pack =>
            {
                pack.UsePack(provider);
                pack.UsePack(app);
            });

            // 异常处理插件
            app.UseGlobalExceptionHandler();

            return app;
        }
    }
}
