using System;
using System.Collections.Generic;
using System.Text;

namespace Lead.Api.DataModel.Models
{
    public class Lead
    {
        public long LeadID { get; set; }

        public string Name { get; set; }

        public string ZipCode { get; set; }
    }
}
