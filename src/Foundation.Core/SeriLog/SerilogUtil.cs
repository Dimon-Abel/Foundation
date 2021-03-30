using Foundation.Core.Extensions;
using Foundation.Core.Utility;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foundation.Core.SeriLog
{
    public class SerilogUtil
    {
        private static bool isInit;

        public static void CreateLogger(string categoryName)
        {
            if (!isInit)
            {
                isInit = true;
                var logConfig = new LoggerConfiguration()
                    //最小的日志输出级别
                    .MinimumLevel.Debug()
                    // 日志调用类命名空间如果以 Microsoft 开头，覆盖日志输出最小级别为 Information
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                    .Enrich.FromLogContext();

                var config = AppConfigManager.GetConfiguration();
                var isWriteToFile = config.GetSection("Logging:WriteToFile")?.Value.CastTo(true) ?? false;
                if (isWriteToFile)
                {
                    // 配置日志输出到文件，文件输出到当前项目的 logs 目录下
                    logConfig.WriteTo.File(Path.Combine("logs", "log.txt"), rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine} {Exception}");
                }
                var elasticSearchServer = config.GetSection("Logging:ElasticSearch:Server")?.Value;
                if (!elasticSearchServer.IsNullOrEmpty())
                {
                    var indexName = config.GetSection("Logging:ElasticSearch:IndexName").Value;
                    var opt = new ElasticsearchSinkOptions(new Uri(elasticSearchServer))
                    {
                        AutoRegisterTemplate = true,
                        OverwriteTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                        IndexFormat = indexName + "{0:yyyyMMdd}",
                        IndexAliases = new string[] { indexName }
                    };
                    opt.TypeName = indexName;
                    logConfig.WriteTo.Elasticsearch(opt);
                }
                Log.Logger = logConfig.CreateLogger();
            }
        }
    }
}
