using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Core.SeriLog
{
    public class LogManage
    {
        private static ILoggerFactory _loggerFactory;
        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_loggerFactory != null)
                {
                    return _loggerFactory;
                }
                else
                {
                    _loggerFactory = new LoggerFactory();
                    _loggerFactory.AddProvider(new SerilogProvider());
                    return _loggerFactory;
                }
            }
        }
        public static ILogger GetLogger<T>()
        {
            return LoggerFactory.CreateLogger<T>();
        }
        public static ILogger GetLogger(string name)
        {
            return LoggerFactory.CreateLogger(name);
        }
        public static ILogger GetLogger(Type type)
        {
            return LoggerFactory.CreateLogger(type);
        }
    }
}