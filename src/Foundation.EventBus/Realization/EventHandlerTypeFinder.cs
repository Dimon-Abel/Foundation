using Foundation.Core.Extensions;
using Foundation.EventBus.Interface;
using Foundation.Server.Reflection.Base;
using Foundation.Server.Reflection.Interface;
using System;
using System.Linq;

namespace Foundation.EventBus.Realization
{
    /// <summary>
    ///     事件处理器类型查找器
    /// </summary>
    public class EventHandlerTypeFinder : FinderBase<Type>, IEventHandlerTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;
        /// <summary>
        ///     初始化一个<see cref="EventHandlerTypeFinder" />类型的新实例
        /// </summary>
        public EventHandlerTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        /// <summary>
        ///     重写以实现所有项的查找
        /// </summary>
        /// <returns></returns>
        protected override Type[] FindAllItems()
        {
            var baseType = typeof(IEventHandler<>);

            var allItems = _allAssemblyFinder.FindAll(true).SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsDeriveClassFrom(baseType)).Distinct().ToArray();

            return allItems;
        }
    }
}