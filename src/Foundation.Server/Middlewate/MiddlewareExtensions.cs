using Foundation.Server.Middlewate.Realization;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Server.Middlewate
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
