using Lead.Messaging.Commands;
using LeadApi.Client;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lead.ServiceBusHost.Handlers
{
    public class FindLeadHandler : IHandleMessages<FindLead>
    {
        private ILeadClient _leadClient;
        public FindLeadHandler(ILeadClient leadClient)
        {
            _leadClient = leadClient;
        }

        public async Task Handle(FindLead message, IMessageHandlerContext context)
        {
            var lead = await _leadClient.GetLeadAsync(message.LeadID);

            await context.Reply(new FindLeadResponse { LeadID = lead.LeadID, Name = lead.Name, ZipCode = lead.ZipCode });
        }
    }
}
