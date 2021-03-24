using System;

namespace Diomin.Foundation.Server.Dependency
{
    /// <summary>
    ///     定义Engine模块依赖
    /// </summary>
    public class DependsOnPacksAttribute : Attribute
    {
        /// <summary>
        ///     初始化一个 Engine模块依赖<see cref="DependsOnPacksAttribute" />类型的新实例
        /// </summary>
        public DependsOnPacksAttribute(params Type[] dependedModuleTypes)
        {
            DependedModuleTypes = dependedModuleTypes;
        }

        /// <summary>
        ///     获取 当前模块的依赖模块类型集合
        /// </summary>
        public Type[] DependedModuleTypes { get; }
    }
}