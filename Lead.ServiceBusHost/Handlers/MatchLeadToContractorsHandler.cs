using Lead.Messaging.Commands;
using Lead.Messaging.Events;
using LeadApi.Client;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lead.ServiceBusHost.Handlers
{
    public class MatchLeadToContractorsHandler : IHandleMessages<MatchLeadToContractors>
    {
        private readonly ILeadClient _leadClient;

        public MatchLeadToContractorsHandler(ILeadClient leadClient)
        {
            _leadClient = leadClient;
        }

        public async Task Handle(MatchLeadToContractors message, IMessageHandlerContext context)
        {
            var matches = new List<Match>();
            foreach (var contractorID in message.ContractorIDs)
            {
                try
                {
                    var match = await _leadClient.AddMatchAsync(
                        message.LeadID,
                        new AddMatchRequest
                        {
                            Price = 5,
                            LeadID = message.LeadID,
                            ContractorID = contractorID
                        });

                    await context.Publish(new MatchCreated
                    {
                        ContractorID = match.ContractorID,
                        LeadID = match.ContractorID,
                        Price = 5,
                        MatchID = match.MatchID
                    });

                    matches.Add(match);

                }
                catch(ApiException exception)
                {
                    await context.Publish(new MatchCreationFailed { ErrorMessage = exception.Message });
                }
            }

            await context.Reply(new MatchLeadToContractorsResponse
            {
                MatchIDs = matches.Select(m => m.MatchID).ToList()
            });
        }
    }
}
