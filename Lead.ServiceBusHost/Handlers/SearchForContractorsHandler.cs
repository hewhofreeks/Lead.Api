using Fake.AvailabilityClient;
using Lead.Messaging.Commands;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lead.ServiceBusHost.Handlers
{
    public class SearchForContractorsHandler : IHandleMessages<SearchForContractors>
    {
        private readonly IAvailabilityClient _availabilityClient;

        public SearchForContractorsHandler(IAvailabilityClient availabilityClient)
        {
            _availabilityClient = availabilityClient;
        }


        public async Task Handle(SearchForContractors message, IMessageHandlerContext context)
        {
            var searchResults = await _availabilityClient.SearchContractors(message.ZipCode);

            await context.Reply(new SearchForContractorsResponse { AvailableContractors = searchResults });
        }
    }
}
