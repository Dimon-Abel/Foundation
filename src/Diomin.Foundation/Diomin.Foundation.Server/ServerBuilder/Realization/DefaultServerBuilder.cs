using Diomin.Foundation.Core.Data;
using Diomin.Foundation.Core.Utility;
using Diomin.Foundation.Server.Extensions;
using Diomin.Foundation.Server.Packs;
using Diomin.Foundation.Server.Reflection.Interface;
using Diomin.Foundation.Server.ServerBuilder.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Diomin.Foundation.Server.ServerBuilder.Realization
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
