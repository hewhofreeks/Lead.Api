using Lead.Messaging.Commands;
using Lead.Messaging.Events;
using LeadApi.Client;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lead.ServiceBusHost.Handlers
{
    public class AddLeadHandler : IHandleMessages<AddLead>
    {
        private readonly ILeadClient _leadClient;

        public AddLeadHandler(ILeadClient leadClient)
        {
            _leadClient = leadClient;
        }

        public async Task Handle(AddLead message, IMessageHandlerContext context)
        {
            var response = await _leadClient.AddLeadAsync(
                new LeadApi.Client.Lead { Name = message.Name, ZipCode = message.ZipCode });

            await context.Publish(new LeadAddedSuccessfully { LeadID = response.LeadID });
        }
    }
}
