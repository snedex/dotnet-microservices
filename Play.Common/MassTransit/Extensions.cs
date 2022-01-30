using System.Reflection;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection services)
        {
            services.AddMassTransit(transitConfig => {

                transitConfig.AddConsumers(Assembly.GetEntryAssembly());

                transitConfig.UsingRabbitMq((context, rabbitConfig) => {

                    var appConfig = context.GetService<IConfiguration>();
                    var serviceSettings = appConfig.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    var settings = appConfig.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();

                    rabbitConfig.Host(settings.Host);
                    rabbitConfig.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                });
            });

            //Start the mass transit message queue bus
            services.AddMassTransitHostedService();

            return services;
        }
    }
}