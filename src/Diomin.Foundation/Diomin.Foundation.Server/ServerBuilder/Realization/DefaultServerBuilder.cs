using Foundation.Core.Data;
using Foundation.Core.Utility;
using Foundation.Server.Extensions;
using Foundation.Server.Packs;
using Foundation.Server.Reflection.Interface;
using Foundation.Server.ServerBuilder.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Server.ServerBuilder.Realization
{
    public class DefaultServerBuilder : IServerBuilder
    {
        public DefaultServerBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            IConfiguration configuration = services.GetSingletonInstanceOrNull<IConfiguration>();
            if (configuration == null)
            {
                configuration = AppConfigManager.GetConfiguration();
                services.AddSingleton(configuration);
            }
            if (configuration != null)
            {
                Singleton<IConfiguration>.Instance = configuration;
            }
        }
        public IServiceCollection Services { get; }

        public List<PackBase> Packs { get; private set; }

        public IServerBuilder Build()
        {
            var allAssemblyFinder = Singleton<IAllAssemblyFinder>.Instance;
            var _typeFinder = new PackTypeFinder(allAssemblyFinder);
            var packTypes = _typeFinder.FindAll();
            Packs = packTypes.Select(m => (PackBase)Activator.CreateInstance(m)).OrderBy(m => m.Level).ThenBy(m => m.Order).ToList();
            foreach (var pack in Packs)
                pack.AddServices(Services);
            return this;
        }
    }
}
