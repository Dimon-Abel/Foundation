using Grains.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
    public class DefaultGrain : IBase
    {
        private readonly ILogger<DefaultGrain> _logger;

        public DefaultGrain(ILogger<DefaultGrain> logger) => _logger = logger;

        public Task Initialization()
        {
            _logger.LogInformation("Grain Initialization Start");
            return Task.CompletedTask;
        }

        public Task<string> Return(string returnStr)
        {
            _logger.LogInformation($"Return: {returnStr}");
            return Task.FromResult(returnStr);
        }
    }
}
