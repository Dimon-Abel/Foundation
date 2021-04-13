using Microsoft.AspNetCore.Http;

namespace Foundation.Server.Host.Interface
{
    /// <summary>
    /// 终结点路由
    /// </summary>
    public interface IEndpointRouter
    {
        /// <summary>
        /// 查找匹配的终结点
        /// </summary>
        /// <param name="context">当前Http请求上下文</param>
        /// <returns></returns>
        IEndpointHandler Find(HttpContext context);
    }
}
