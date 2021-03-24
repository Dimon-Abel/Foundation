using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Server.Dependency.Interface
{
    /// <summary>
    ///     实现此接口的类型将被注册为<see cref="ServiceLifetime.Scoped" />模式
    /// </summary>
    [IgnoreDependency]
    public interface IScopeDependency
    {
    }
}
