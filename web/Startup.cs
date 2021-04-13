using Foundation.RabbitMQ.Extensions;
using Foundation.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundation.WebApi
{
    /// <summary>
    /// 程序启动项
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        /// <summary>
        /// 配置文件
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // 添加RabbmitMQ
            services.AddRabbitMQMessagBus(Configuration);
            // 添加基础框架服务
            services.AddFoundationServer();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseAuthorization();
            // 添加基础框架
            app.UseFoundationServer()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
