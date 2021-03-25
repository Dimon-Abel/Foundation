using Foundation.Core.Extensions;
using Foundation.Core.SeriLog;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Foundation.Server.Middlewate.Realization
{
    public class GlobalExceptionHandlerMiddleware : Interface.IMiddleware
    {
        private RequestDelegate _next;
        private readonly ILogger _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = LogManage.GetLogger<GlobalExceptionHandlerMiddleware>(); //loggerFactory.CreateLogger<GlobalExceptionHandlerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var clientIp = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var userAgent = context.Request.Headers["User-Agent"];
                _logger.LogError(ex, $"客户端IP“{clientIp}”请求“{context.Request.Path}”时出错：{ex.Message}；UserAgent：{userAgent}");
                //仅在代码请求时输出json异常数据
                if (context.Request.IsAjaxRequest() || context.Request.IsJsonContextType())
                {
                    if (context.Response.HasStarted)
                    {
                        return;
                    }
                    context.Response.StatusCode = 500;
                    context.Response.Clear();
                    context.Response.ContentType = "application/json; charset=utf-8";
                    context.Response.Headers["access-control-allow-origin"] = "*";
                    context.Response.Headers["access-control-allow-headers"] = "authorization";
                    await context.Response.WriteAsync(new
                    {
                        code = 9,
                        data = string.Empty,
                        message = ex.Message
                    }.ToJsonString());
                    return;
                }
                throw;
            }
        }
    }
}