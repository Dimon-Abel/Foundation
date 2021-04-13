using Foundation.Core.Extensions;
using Foundation.Server.Domain.Interface;
using Foundation.Server.Reflection.Base;
using Foundation.Server.Reflection.Interface;
using System;
using System.Linq;

namespace Foundation.Server.Domain.Reflection
{
    /// <summary>
    ///     <see cref="IOutputDto" />类型查找器
    /// </summary>
    public class OutputDtoTypeFinder : BaseTypeFinderBase<IOutputDto<Type>>, IOutputDtoTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        /// <summary>
        ///     初始化一个<see cref="BaseTypeFinderBase{TBaseType}" />类型的新实例
        /// </summary>
        public OutputDtoTypeFinder(IAllAssemblyFinder allAssemblyFinder)
            : base(allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        /// <summary>
        ///     重写以实现所有项的查找
        /// </summary>
        /// <returns></returns>
        protected override Type[] FindAllItems()
        {
            var assemblies = _allAssemblyFinder.FindAll(true);
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsDeriveClassFrom(typeof(IOutputDto<>))).Distinct().ToArray();
        }
    }
}