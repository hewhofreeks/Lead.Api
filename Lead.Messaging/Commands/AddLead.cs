using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lead.Messaging.Commands
{
    public class AddLead : ICommand
    {
        public string Name { get; set; }

        public string ZipCode { get; set; }
    }
}
