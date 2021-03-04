using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lead.Messaging.Commands
{
    public class MatchLeadToContractors : ICommand
    {
        public long LeadID { get; set; }

        public List<long> ContractorIDs { get; set; }

        public DateTimeOffset TimeSent { get; set; } = DateTimeOffset.Now;
    }

    public class MatchLeadToContractorsResponse : IMessage
    {
        public List<long> MatchIDs { get; set; }
        public DateTimeOffset TimeSent { get; set; } = DateTimeOffset.Now;
    }
}
