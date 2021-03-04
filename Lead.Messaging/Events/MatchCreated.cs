using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lead.Messaging.Events
{
    public class MatchCreated : IEvent
    {
        public long LeadID { get; set; }
        public long ContractorID { get; set; }
        public long MatchID { get; set; }
        public decimal Price { get; set; }

        public DateTimeOffset TimeSent { get; set; } = DateTimeOffset.Now;
    }
}
