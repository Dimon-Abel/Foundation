using Diomin.Foundation.Server.Dependency;
using System;

namespace Diomin.Foundation.Server.Reflection.Interface
{
    /// <summary>
    ///     定义类型查找行为
    /// </summary>
    [IgnoreDependency]
    public interface ITypeFinder : IFinder<Type>
    {
    }
}
