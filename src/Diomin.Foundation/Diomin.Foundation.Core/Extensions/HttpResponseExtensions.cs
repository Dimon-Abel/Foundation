using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Core.Extensions
{
    public static class HttpResponseExtensions
    {
        public static async Task WriteJsonAsync(this HttpResponse response, object o, string contentType = null)
        {
            var json = o.ToJson(true); // 驼峰式json字符串
            await response.WriteJsonAsync(json, contentType);
            await response.Body.FlushAsync();
        }

        public static async Task WriteJsonAsync(this HttpResponse response, string json, string contentType = null)
        {
            response.ContentType = contentType ?? "application/json; charset=UTF-8";
            await response.WriteAsync(json);
            await response.Body.FlushAsync();
        }

        public static void SetCache(this HttpResponse response, int maxAge, params string[] varyBy)
        {
            if (maxAge == 0)
            {
                SetNoCache(response);
            }
            else if (maxAge > 0)
            {
                if (!response.Headers.ContainsKey("Cache-Control"))
                {
                    response.Headers.Add("Cache-Control", $"max-age={maxAge}");
                }

                if (varyBy?.Any() == true)
                {
                    var vary = varyBy.Aggregate((x, y) => x + "," + y);
                    if (response.Headers.ContainsKey("Vary"))
                    {
                        vary = response.Headers["Vary"].ToString() + "," + vary;
                    }
                    response.Headers["Vary"] = vary;
                }
            }
        }

        public static void SetNoCache(this HttpResponse response)
        {
            if (!response.Headers.ContainsKey("Cache-Control"))
            {
                response.Headers.Add("Cache-Control", "no-store, no-cache, max-age=0");
            }
            else
            {
                response.Headers["Cache-Control"] = "no-store, no-cache, max-age=0";
            }

            if (!response.Headers.ContainsKey("Pragma"))
            {
                response.Headers.Add("Pragma", "no-cache");
            }
        }

        public static async Task WriteHtmlAsync(this HttpResponse response, string html)
        {
            response.ContentType = "text/html; charset=UTF-8";
            await response.WriteAsync(html, Encoding.UTF8);
            await response.Body.FlushAsync();
        }
    }
}