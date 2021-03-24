using Diomin.Foundation.Server.Reflection.Base;
using Diomin.Foundation.Server.Reflection.Interface;

namespace Diomin.Foundation.Server.Packs
{
    /// <summary>
    ///     Engine模块类型查找器
    /// </summary>
    public class PackTypeFinder : BaseTypeFinderBase<PackBase>, ITypeFinder
    {
        /// <summary>
        ///     初始化一个<see cref="PackTypeFinder" />类型的新实例
        /// </summary>
        public PackTypeFinder(IAllAssemblyFinder allAssemblyFinder)
            : base(allAssemblyFinder)
        {
        }
    }
}
