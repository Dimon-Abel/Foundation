using Foundation.RabbitMQ.Interface;
using Foundation.RabbitMQ.Realization;
using Foundation.Server.Packs;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.RabbitMQ
{
    public class RabbitMQPack : PackBase
    {
        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddTransient<IMQFactoryCenter, MQFactoryCenter>();
            return services;
        }
    }
}
