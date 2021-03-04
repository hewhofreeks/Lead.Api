using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lead.Messaging.Events
{
    public class MatchCreationFailed : IEvent
    {
        public string ErrorMessage { get; set; }
    }
}
