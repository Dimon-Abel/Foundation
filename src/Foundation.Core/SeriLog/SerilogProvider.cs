using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Core.SeriLog
{
    public class SerilogProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            SerilogUtil.CreateLogger(categoryName);
            return new SerilogLogger(categoryName);
        }

        public void Dispose() { }
    }
}