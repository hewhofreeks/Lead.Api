using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Lead.Api.DataModel.Models
{
    public class Match
    {
        [Key]
        public long MatchID { get; set; }

        public decimal PriceOfLead { get; set; }

        public long LeadID { get; set; }

        public long ContractorID { get; set; }
    }
}
