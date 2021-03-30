using Foundation.Core.SeriLog;
using Foundation.Server.Packs;
using Foundation.Server.Packs.Enum;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Foundation.Core.Utility;
using Foundation.Server.Dependency;

namespace Foundation.Server.BasePacks
{
    /*
     "Swagger": {
      "Title": "接口名称",
      "Version": 1,
      "Enabled": true,
    }
     */
    /// <summary>
    /// swagger 配置
    /// </summary>
    public sealed class SwaggerPack : PackBase
    {
        /// <summary>
        ///     获取 模块级别，级别越小越先启动
        /// </summary>
        public override PackLevel Level => PackLevel.Framework;
        /// <summary>
        ///  日志
        /// </summary>
        private ILogger _logger = LogManage.GetLogger<SwaggerPack>();

        private FoundationSwaggerOptions _options;
        /// <summary>
        ///     获取 模块启动顺序，模块启动的顺序先按级别启动，同一级别内部再按此顺序启动，
        ///     级别默认为0，表示无依赖，需要在同级别有依赖顺序的时候，再重写为>0的顺序值
        /// </summary>
        public override int Order => 2;
        public override IServiceCollection AddServices(IServiceCollection services)
        {
            _options = AppConfigManager.GetSection<FoundationSwaggerOptions>("SwaggerOptions");
            if (_options != null)
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc($"v{_options.Version}", new OpenApiInfo() { Title = _options.Title, Version = $"v{_options.Version}" });
                    Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml").ToList().ForEach(file => { options.IncludeXmlComments(file); });
                });
            }
            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v{_options.Version}/swagger.json", $"{_options.Title} V{_options.Version}");
            });
        }

        public class FoundationSwaggerOptions
        {
            public string Title { get; set; }
            public int Version { get; set; }
            public bool Enabled { get; set; }
        }
    }
}
