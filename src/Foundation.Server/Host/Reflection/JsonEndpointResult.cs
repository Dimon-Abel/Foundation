using Foundation.Core.Entity;
using Foundation.Core.Extensions;
using Foundation.Server.Host.Interface;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Foundation.Server.Host.Reflection
{
    /// <summary>
    /// Json类型的终结点结果对象
    /// </summary>
    public class JsonEndpointResult : ReturnResult, IEndpointResult
    {
        public JsonEndpointResult() : base() { }
        public JsonEndpointResult(string data) : base(ReturnResultCode.Success, data)
        {
        }
        public JsonEndpointResult(ReturnResultCode code, string message) : base(code, null, message)
        {
        }
        public JsonEndpointResult(ReturnResultCode code, string message, string data) : base(code, data, message)
        {
        }
        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.SetNoCache();
            await context.Response.WriteJsonAsync(this);
        }
        /// <summary>
        /// 输出异常信息
        /// </summary>
        /// <param name="message"></param>
        public static JsonEndpointResult Error(string message)
        {
            return new JsonEndpointResult(ReturnResultCode.Error, message);
        }
    }
}
