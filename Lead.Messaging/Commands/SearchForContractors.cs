using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lead.Messaging.Commands
{
    public class SearchForContractors : ICommand
    {
        public string ZipCode { get; set; }

        public DateTimeOffset TimeSent { get; set; } = DateTimeOffset.Now;
    }

    public class SearchForContractorsResponse : IMessage
    {
        public IEnumerable<long> AvailableContractors { get; set; }

        public DateTimeOffset TimeSent { get; set; } = DateTimeOffset.Now;
    }
}
