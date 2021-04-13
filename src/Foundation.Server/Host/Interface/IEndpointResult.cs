using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Foundation.Server.Host.Interface
{
    /// <summary>
    /// 终结点结果
    /// </summary>
    public interface IEndpointResult
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        /// <param name="context">当前Http请求上下文</param>
        /// <returns></returns>
        Task ExecuteAsync(HttpContext context);
    }
}
