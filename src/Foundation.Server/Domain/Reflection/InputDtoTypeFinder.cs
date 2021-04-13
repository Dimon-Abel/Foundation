using Foundation.Core.Extensions;
using Foundation.Server.Domain.Interface;
using Foundation.Server.Reflection.Base;
using Foundation.Server.Reflection.Interface;
using System;
using System.Linq;

namespace Foundation.Server.Domain.Reflection
{
    /// <summary>
    ///     <see cref="IInputDto{T}" />类型查找器
    /// </summary>
    public class InputDtoTypeFinder : FinderBase<Type>, IInputDtoTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        /// <summary>
        ///     初始化一个<see cref="InputDtoTypeFinder" />类型的新实例
        /// </summary>
        public InputDtoTypeFinder(IAllAssemblyFinder allAssemblyFinder)
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
                .Where(type => type.IsDeriveClassFrom(typeof(IInputDto<>))).Distinct().ToArray();
        }
    }
}