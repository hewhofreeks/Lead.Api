using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lead.Messaging.Commands;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace Lead.Api
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
                    var endpointConfiguration = new EndpointConfiguration("Api.ServiceBusHost");

                    var transport = endpointConfiguration.UseTransport<LearningTransport>();

                    transport.Routing().RouteToEndpoint(typeof(AddLead), "Lead.ServiceBusHost");
                    
                    endpointConfiguration.UsePersistence<LearningPersistence>();
                    endpointConfiguration.SendOnly();

                    return endpointConfiguration;
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
