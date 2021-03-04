using Lead.Api.DataModel;
using Lead.Api.DataModel.Models;
using Lead.Api.Models;
using Lead.Messaging.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModels = Lead.Api.DataModel;

namespace Lead.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeadController : ControllerBase
    {
        private readonly LeadContext _context;
        private readonly IMessageSession _messageSession;

        public LeadController(LeadContext leadContext, IMessageSession messageSession)
        {
            _context = leadContext;
            _messageSession = messageSession;
        }

        [HttpGet("")]
        public async Task<IEnumerable<DataModels.Models.Lead>> GetLeads()
        {
            return await _context.Leads.AsQueryable().ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<DataModels.Models.Lead> GetLead([FromRoute] long id)
        {
            return await _context.Leads.FirstOrDefaultAsync(l => l.LeadID == id);
        }

        [HttpPost("AddFromMessaging")]
        public async Task AddLeadFromMessaging([FromBody] DataModels.Models.Lead lead)
        {
            await _messageSession.Send(new AddLead { Name = lead.Name, ZipCode = lead.ZipCode });
        }

        [HttpPost("")]
        public async Task<DataModels.Models.Lead> AddLead([FromBody] DataModels.Models.Lead lead)
        {
            _context.Add(lead);

            await _context.SaveChangesAsync();

            return lead;
        }

        [HttpGet("{leadID}/Match")]
        public async Task<IEnumerable<Match>> GetMatches([FromRoute]long leadID)
        {
            return await _context.Matches.Where(m => m.LeadID == leadID).ToListAsync();
        }

        [HttpPost("{leadID}/Match")]
        public async Task<Match> AddMatch([FromRoute] long leadID, [FromBody] AddMatchRequest request)
        {
            var lead = await _context.Leads.FirstOrDefaultAsync(l => l.LeadID == leadID);
            if (lead == null)
                throw new KeyNotFoundException($"Lead: {leadID} not found");

            var contractor = await _context.Contractors.FirstOrDefaultAsync(c => c.ContractorID == request.ContractorID);

            if (contractor == null)
                throw new KeyNotFoundException($"Contractor: {request.ContractorID} not found");

            var match = new Match { ContractorID = request.ContractorID, LeadID = leadID, PriceOfLead = request.Price };

            _context.Add(match);

            await _context.SaveChangesAsync();

            return match;
        }
    }
}
