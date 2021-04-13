using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Server.Host.Interface
{
    /// <summary>
    /// 终结点处理对象
    /// </summary>
    public interface IEndpointHandler
    {
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context">当前Http请求上下文</param>
        /// <returns></returns>
        Task<IEndpointResult> ProcessAsync(HttpContext context);
    }
}
