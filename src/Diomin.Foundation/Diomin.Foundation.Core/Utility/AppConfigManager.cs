using Foundation.Core.Extensions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Foundation.Core.Utility
{
    /// <summary>
    /// Appsetting.Json管理器
    /// </summary>
    public static class AppConfigManager
    {
        private static IConfiguration configuration;
        static AppConfigManager()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", true);
            configuration = builder.Build();
        }


        /// <summary>
        /// 获取appsettings.json配置文本中指定key的值 
        /// </summary>
        /// <param name="key">节点名称，多节点以:分隔</param>
        public static string Get(string key)
        {
            return configuration[key];
        }
        /// <summary>
        /// 获取appsetting.json配置对象
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetConfiguration()
        {
            return configuration;
        }

        /// <summary>
        /// 获取appsettings.json配置文本中指定key的值 
        /// <typeparamref name="T">转换类型</typeparamref>
        /// </summary>
        public static T Get<T>(string key)
        {
            string value = Get(key);
            if (string.IsNullOrEmpty(value))
                return default(T);
            if (!value.Contains("{"))
                return value.CastTo<T>();

            return value.FromJson<T>();
        }

        public static T GetInstance<T>(string key, T instance)
        {
            var config = configuration.GetSection(key);
            if (!config.Exists())
            {
                return default;
            }
            config.Bind(instance);
            return instance;
        }
    }
}
