using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lead.Messaging.Commands
{
    public class FindLead : ICommand
    {
        public long LeadID { get; set; }

        public DateTimeOffset TimeSent { get; set; } = DateTimeOffset.Now;
    }

    public class FindLeadResponse : IMessage
    {
        public long LeadID { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
    }
}
