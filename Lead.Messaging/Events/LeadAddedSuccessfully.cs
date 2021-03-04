using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace Lead.Messaging.Events
{
    public class LeadAddedSuccessfully : IEvent
    {
        public long LeadID { get; set; }
    }
}
