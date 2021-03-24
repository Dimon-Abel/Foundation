using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.Web
{
    /// <summary>
    /// 请求上下文
    /// </summary>
    public static class HttpContext
    {
        private static IHttpContextAccessor m_httpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                return m_httpContextAccessor?.HttpContext;
            }
        }
        /// <summary>
        /// 获取请求头部指定键的值
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string[] GetValues(this IHeaderDictionary headers, string key)
        {
            if (headers.ContainsKey(key))
                return headers[key];
            return null;
        }
    }
}
