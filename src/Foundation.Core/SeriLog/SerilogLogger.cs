using Foundation.Core.Extensions;
using Foundation.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Core.SeriLog
{
    public class SerilogLogger : Microsoft.Extensions.Logging.ILogger
    {
        private string _level;
        private string _typeName;
        public SerilogLogger(string typeName)
        {
            _typeName = typeName;
            var config = AppConfigManager.GetConfiguration();
            _level = config.GetSection("Logging:LogLevel").Value ?? config.GetSection("Logging:LogLevel:Default").Value;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return SerilogDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var level = _level.IsNullOrEmpty() ? LogLevel.Debug : _level.CastTo<LogLevel>();
            return level <= logLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message) && exception != null)
            {
                message = exception.ToString();
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            switch (logLevel)
            {
                case LogLevel.None:
                    break;
                case LogLevel.Trace:
                case LogLevel.Debug:
                    Serilog.Log.Debug(_typeName + " " + message);
                    break;
                case LogLevel.Information:
                    Serilog.Log.Information(_typeName + " " + message);
                    break;
                case LogLevel.Warning:
                    Serilog.Log.Warning(_typeName + " " + message);
                    break;
                case LogLevel.Error:
                    Serilog.Log.Error(exception, message);
                    break;
                case LogLevel.Critical:
                    Serilog.Log.Fatal(message);
                    break;
                default:
                    Serilog.Log.Warning($"遇到未知的日志级别 {logLevel}, 使用Info级别写入日志。");
                    break;
            }
        }
        private class SerilogDisposable : IDisposable
        {
            public static SerilogDisposable Instance = new SerilogDisposable();
            public void Dispose() { }
        }
    }

}