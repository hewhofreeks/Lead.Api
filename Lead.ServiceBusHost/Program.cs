using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fake.AvailabilityClient;
using LeadApi.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace Lead.ServiceBusHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseNServiceBus(context =>
                {
                    var endpointConfiguration = new EndpointConfiguration("Lead.ServiceBusHost");
                    
                    endpointConfiguration.UseTransport<LearningTransport>(); // Would be azure for us
                    endpointConfiguration.UsePersistence<LearningPersistence>(); // Would be azure for us

                    return endpointConfiguration;
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.AddHttpClient<ILeadClient, LeadClient>(options => 
                    {
                        options.BaseAddress = new Uri(hostContext.Configuration["Lead.Api.Url"]);
                    });

                    services.AddTransient<IAvailabilityClient, AvailabilityClient>();
                });
    }
}
