using Foundation.Dapper.Interface;
using Foundation.Dapper.Realization;
using Foundation.Server.Packs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Dapper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDapper(IServiceCollection services)
        {
            services.AddTransient<IDapperExecute, DapperExecute>();
            return services;
        }
    }
}
