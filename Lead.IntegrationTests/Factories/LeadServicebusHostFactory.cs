using Fake.AvailabilityClient;
using LeadApi.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NServiceBus;
using NServiceBus.Extensions.IntegrationTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lead.IntegrationTests.Factories
{
    public class LeadServicebusHostFactory : WebApplicationFactory<Lead.ServiceBusHost.Program>
    {
        private ILeadClient _leadClient;

        public LeadServicebusHostFactory(ILeadClient leadClient)
        {
            _leadClient = leadClient;
        }

        protected override IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .UseNServiceBus(ctxt =>
            {
                var endpoint = new EndpointConfiguration("LeadService.FactoryTests");

                endpoint.ConfigureTestEndpoint();
                endpoint.UseTransport<LearningTransport>();
                endpoint.UsePersistence<LearningPersistence>();

                return endpoint;
            })
            .ConfigureServices(services =>
            {
                services.AddTransient<ILeadClient>((c) =>
                {
                    return _leadClient;
                });

                services.AddTransient<IAvailabilityClient>(c => 
                {
                    var clientMock = new Mock<IAvailabilityClient>();

                    List<long> fakeContractorIDs = new List<long>() { 1, 2, 3, 4 };
                    

                    clientMock.Setup(e => e.SearchContractors(It.IsAny<string>()))
                        .Returns<string>((zip) => 
                        {
                            if(zip == "12345")    
                                return Task.FromResult(new List<long>());

                            return Task.FromResult(fakeContractorIDs);
                        });

                    return clientMock.Object;
                });
            })
            .ConfigureWebHostDefaults(b => b.Configure(app => { }));

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("integration");
            //builder.UseContentRoot(Directory.GetCurrentDirectory());

            return base.CreateHost(builder);
        }
    }
}
