using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lead.Api;
using Lead.Api.DataModel;
using Lead.Messaging.Commands;
using LeadApi.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using NServiceBus.Extensions.IntegrationTesting;

namespace Lead.IntegrationTests.Factories
{
    public class LeadApiFactory : WebApplicationFactory<Startup>
    {
        private ILeadClient _leadClient;

        public ILeadClient CreateLeadClient()
        {
            if (_leadClient != null) 
                return _leadClient;

            var httpClient = CreateClient();

            _leadClient = new LeadClient(httpClient);

            return _leadClient;
        }

        public async Task SeedDatabase(Func<LeadContext, Task> contextSeed)
        {
            await contextSeed(this.Services.GetService<LeadContext>());
        }

        public void CleanupDB()
        {
           var context = this.Services.GetService<LeadContext>();

            context.Matches.RemoveRange(context.Matches);
            context.Leads.RemoveRange(context.Leads);
            context.Contractors.RemoveRange(context.Contractors);
            context.SaveChanges();
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .UseNServiceBus(c =>
                {
                    var endpoint = new EndpointConfiguration("Api.FactoryTests");

                    var transport = endpoint.UseTransport<LearningTransport>();

                    transport.Routing().RouteToEndpoint(typeof(AddLead), "LeadService.FactoryTests");

                    endpoint.UsePersistence<LearningPersistence>();
                    endpoint.SendOnly();

                    return endpoint;
                })
                .ConfigureWebHostDefaults(b => b.UseStartup<Startup>());
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("integration");
            //builder.UseContentRoot(Directory.GetCurrentDirectory());

            builder.ConfigureTestServices(services => 
            {
                // Configure other service overrides
                services.AddDbContext<LeadContext>(options => {
                    options.UseInMemoryDatabase("LeadDB");
                });
            });
        }
    }
}
