using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Server.Dependency.Interface
{
    /// <summary>
    /// 包装微软的IServiceScopeFactory
    /// </summary>
    public interface IHybridServiceScopeFactory : IServiceScopeFactory
    {
    }
}
