using Foundation.Server.Reflection.Base;
using Foundation.Server.Reflection.Interface;

namespace Foundation.Server.Packs
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
