using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lead.Messaging.Events
{
    public class MatchingProcessFinished : IEvent
    {
        public List<long> MatchIDs { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
        public long LeadID { get; set; }

        public DateTimeOffset TimeSent { get; set; } = DateTimeOffset.Now;
    }
}
