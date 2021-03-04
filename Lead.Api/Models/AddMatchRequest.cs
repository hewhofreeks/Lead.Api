using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lead.Api.Models
{
    public class AddMatchRequest
    {
        public long LeadID { get; set; }
        public long ContractorID { get; set; }
        public decimal Price { get; set; }
    }
}
