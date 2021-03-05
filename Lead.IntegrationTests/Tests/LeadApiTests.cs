using Lead.IntegrationTests.Factories;
using LeadApi.Client;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lead.IntegrationTests.Tests
{
    [TestFixture]
    [SingleThreaded]
    public class LeadApiTests
    {
        private readonly LeadApiFactory _apiFactory;

        public LeadApiTests()
        {
            _apiFactory = new LeadApiFactory();
        }

        [TearDown]
        public async Task CleanUp()
        {
            _apiFactory.CleanupDB();
        }

        [Test]
        public async Task Test_AddLead()
        {
            // Arrange
            //var apiFactory = new LeadApiFactory();
            await _apiFactory.SeedDatabase(async context => 
            {
                await context.SaveChangesAsync();
            });

            var client = _apiFactory.CreateLeadClient();
            var lead = new LeadApi.Client.Lead { Name = "Phill", ZipCode = "60640" };

            // Act
            var result = await client.AddLeadAsync(lead);

            // Assert
            Assert.IsTrue(result.LeadID > 0);
            Assert.AreEqual(result.Name, lead.Name);
            Assert.AreEqual(result.ZipCode, lead.ZipCode);
        }

        [Test]
        public async Task Test_CreateMatch()
        {
            // Arrange
            await _apiFactory.SeedDatabase(async context =>
            {
                context.Contractors.Add(new Lead.Api.DataModel.Models.Contractor { ContractorID = 5, CompanyName = "Test Contractor" });
                context.Leads.Add(new Lead.Api.DataModel.Models.Lead { LeadID = 1, Name = "Phill", ZipCode = "60640" });

                await context.SaveChangesAsync();
            });
            var client = _apiFactory.CreateLeadClient();
            var matchRequest = new AddMatchRequest { LeadID = 1, ContractorID = 5, Price = 5 };

            // Act
            var match = await client.AddMatchAsync(matchRequest.LeadID, matchRequest);

            // Assert
            Assert.IsTrue(match.MatchID > 0);
            Assert.AreEqual(1, match.LeadID);
            Assert.AreEqual(5, match.ContractorID);
        }

        [Test]
        public async Task Test_CreateMatch_FailsWhenInvalidLeadID()
        {
            // Arrange
            await _apiFactory.SeedDatabase(async context =>
            {
                context.Contractors.Add(new Lead.Api.DataModel.Models.Contractor { ContractorID = 5, CompanyName = "Test Contractor" });
                context.Leads.Add(new Lead.Api.DataModel.Models.Lead { LeadID = 1, Name = "Phill", ZipCode = "60640" });

                await context.SaveChangesAsync();
            });
            var client = _apiFactory.CreateLeadClient();
            var matchRequest = new AddMatchRequest { LeadID = 2, ContractorID = 5, Price = 5 };
            ApiException expectedException = null;

            // Act
            try
            {
                var match = await client.AddMatchAsync(matchRequest.LeadID, matchRequest);
            }
            catch(ApiException exception)
            {
                expectedException = exception;
            }

            // Assert
            Assert.IsNotNull(expectedException);
            Assert.AreEqual(StatusCodes.Status400BadRequest, expectedException.StatusCode);
        }
    }
}

