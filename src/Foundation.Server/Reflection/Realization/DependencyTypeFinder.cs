using Foundation.Core.Extensions;
using Foundation.Server.Dependency;
using Foundation.Server.Dependency.Interface;
using Foundation.Server.Reflection.Base;
using Foundation.Server.Reflection.Interface;
using System;
using System.Linq;

namespace Foundation.Server.Reflection.Realization
{
    /// <summary>
    /// 依赖注入类型查找器
    /// </summary>
    public class DependencyTypeFinder : FinderBase<Type>, IDependencyTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        /// <summary>
        /// 初始化一个<see cref="DependencyTypeFinder"/>类型的新实例
        /// </summary>
        public DependencyTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        /// <summary>
        /// 重写以实现所有项的查找
        /// </summary>
        /// <returns></returns>
        protected override Type[] FindAllItems()
        {
            Type[] baseTypes = new[] { typeof(ISingletonDependency), typeof(IScopeDependency), typeof(ITransientDependency) };
            Type[] types = _allAssemblyFinder.FindAll(true).SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface && !type.HasAttribute<IgnoreDependencyAttribute>()
                    && (baseTypes.Any(b => b.IsAssignableFrom(type)) || type.HasAttribute<DependencyAttribute>()))
                .ToArray();
            return types;
        }
    }
}