using Lead.Messaging.Commands;
using Lead.Messaging.Events;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lead.ServiceBusHost.Sagas
{
    public class LeadMatchingProcessSaga : Saga<LeadMatchingData>, 
        IAmStartedByMessages<LeadAddedSuccessfully>,
        IHandleMessages<FindLeadResponse>,
        IHandleMessages<SearchForContractorsResponse>,
        IHandleMessages<MatchLeadToContractorsResponse>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<LeadMatchingData> mapper)
        {
            mapper.ConfigureMapping<LeadAddedSuccessfully>(c => c.LeadID).ToSaga(c => c.LeadID);
        }

        public async Task Handle(LeadAddedSuccessfully message, IMessageHandlerContext context)
        {
            await context.SendLocal(new FindLead { LeadID = message.LeadID });
        }
        public async Task Handle(FindLeadResponse message, IMessageHandlerContext context)
        {
            Data.Name = message.Name;
            Data.ZipCode = message.ZipCode;

            await context.SendLocal(new SearchForContractors { ZipCode = message.ZipCode });
        }

        public async Task Handle(SearchForContractorsResponse message, IMessageHandlerContext context)
        {
            Data.ContractorIDs = message.AvailableContractors.ToList();

            await context.SendLocal(new MatchLeadToContractors { ContractorIDs = Data.ContractorIDs, LeadID = Data.LeadID });
        }

        public async Task Handle(MatchLeadToContractorsResponse message, IMessageHandlerContext context)
        {
            await context.Publish(new MatchingProcessFinished
            {
                LeadID = Data.LeadID,
                MatchIDs = message.MatchIDs,
                Name = Data.Name,
                ZipCode = Data.ZipCode
            });
        }
    }

    public class LeadMatchingData : ContainSagaData
    {
        public long LeadID { get; set; }

        public string Name { get; set; }

        public string ZipCode { get; set; }

        public List<long> ContractorIDs { get; set; } = new List<long>();
    }
}
