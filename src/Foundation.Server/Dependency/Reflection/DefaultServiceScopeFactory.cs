using Foundation.Server.Dependency.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Server.Dependency.Reflection
{
    /// < summary >
    ///默认< see  cref = " IServiceScope " />工厂，行为和< see  cref = " IServiceScopeFactory " />一样
    /// </ summary >
    [Dependency(ServiceLifetime.Singleton, TryAdd = true)]
    public class DefaultServiceScopeFactory : IHybridServiceScopeFactory
    {
        /// < summary >
        ///初始化一个< see  cref = " DefaultServiceScopeFactory " />类型的新实例
        /// </ summary >
        public DefaultServiceScopeFactory(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = serviceScopeFactory;
        }

        /// < summary >
        ///获取< see  cref = " IServiceScope " />工厂
        /// </ summary >
        protected IServiceScopeFactory ServiceScopeFactory { get; }

        #region  Implementation of IServiceScopeFactory 微软源码实现

        /// < summary >
        /// 创建一个<see cref=“t:microsoft.extensions.dependencyInjection.ISeviceScope”/>
        /// 包含用于创建新的作用域解析依赖项的 < see  cref = " T:System.IServiceProvider " />作用域
        /// </ summary >
        /// < returns >
        /// 控制作用域< see  cref = " T:Microsoft.Extensions.DependencyInjection.IServiceScope " />的生存期。
        /// 一旦释放了它，任何已从< see  cref = " P:Microsoft.Extensions.DependencyInjection.IServiceScope.ServiceProvider " />解析的作用域服务也将被释放。
        /// </ returns >
        public IServiceScope CreateScope()
        {
            return ServiceScopeFactory.CreateScope();
        }

        # endregion
    }
}