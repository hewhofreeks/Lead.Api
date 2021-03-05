using Lead.Api.DataModel;
using Lead.IntegrationTests.Factories;
using Lead.Messaging.Commands;
using LeadApi.Client;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Extensions.IntegrationTesting;
using Lead.Messaging.Events;
using System.Linq;
using Lead.ServiceBusHost.Sagas;
using Lead.IntegrationTests;
using NUnit.Framework;

namespace Lead.IntegrationTests.Tests
{
    [TestFixture]
    [SingleThreaded]
    public class LeadSBHTests
    {
        private LeadApiFactory _apiFactory;
        private LeadServicebusHostFactory _leadServicebusHostFactory;
        private ILeadClient _leadClient;

        public LeadSBHTests()
        {
            
        }

        [OneTimeSetUp]
        public void Init()
        {
            _apiFactory = new LeadApiFactory();
            _leadClient = _apiFactory.CreateLeadClient();
            _leadServicebusHostFactory = new LeadServicebusHostFactory(_leadClient);
        }

        [TearDown]
        public async Task CleanUp()
        {
            _apiFactory.CleanupDB();

            await Task.Delay(1000); // Gotcha due to the ordering of these tests
        }

        private async Task DataSeeds(LeadContext context)
        {
            context.Contractors.Add(new Api.DataModel.Models.Contractor { ContractorID = 1, CompanyName = "Test 1" });
            context.Contractors.Add(new Api.DataModel.Models.Contractor { ContractorID = 2, CompanyName = "Test 2" });
            context.Contractors.Add(new Api.DataModel.Models.Contractor { ContractorID = 3, CompanyName = "Test 3" });
            context.Contractors.Add(new Api.DataModel.Models.Contractor { ContractorID = 4, CompanyName = "Test 4" });
            context.Contractors.Add(new Api.DataModel.Models.Contractor { ContractorID = 5, CompanyName = "Test 5" });

            await context.SaveChangesAsync();
        }

        [Test]
        public async Task Test_AddLeadsCommand_AddsLeadSuccessfully()
        {
            // Arrange
            await _apiFactory.SeedDatabase(DataSeeds);
            var addLeadCommand = new AddLead { Name = "Test Tester", ZipCode = "60640" };
            var session = _leadServicebusHostFactory.Services.GetService<IMessageSession>();

            // Act
            var result = await EndpointFixture.ExecuteAndWaitForHandled<AddLead>(
                () => session.SendLocal(addLeadCommand));

            // Assert
            var successMessages = result.SentMessages.OfType<LeadAddedSuccessfully>();

            Assert.IsTrue(successMessages.Count() > 0);
            Assert.IsTrue(successMessages.First().LeadID > 0);

            var lead = await _leadClient.GetLeadAsync(successMessages.First().LeadID);

            Assert.AreEqual(successMessages.First().LeadID, lead.LeadID);
            Assert.AreEqual(addLeadCommand.Name, lead.Name);
            Assert.AreEqual(addLeadCommand.ZipCode, lead.ZipCode);
        }

        [Test]
        public async Task Test_AddsLead_StartsFromApi()
        {
            // Arrange
            await _apiFactory.SeedDatabase(DataSeeds);
            var addLead = new LeadApi.Client.Lead { Name = "Test Tester 2", ZipCode = "60641" };
            var session = _leadServicebusHostFactory.Services.GetService<IMessageSession>();

            // Act
            var result = await EndpointFixture.ExecuteAndWaitForHandled<AddLead>(
                () => _leadClient.AddLeadFromMessagingAsync(addLead));

            // Assert
            var successMessages = result.SentMessages.OfType<LeadAddedSuccessfully>();

            Assert.IsTrue(successMessages.Count() > 0);
            Assert.IsTrue(successMessages.First().LeadID > 0);

            var lead = await _leadClient.GetLeadAsync(successMessages.First().LeadID);

            Assert.AreEqual(successMessages.First().LeadID, lead.LeadID);
            Assert.AreEqual(addLead.Name, lead.Name);
            Assert.AreEqual(addLead.ZipCode, lead.ZipCode);
        }

        [Test]
        public async Task Test_MatchingSaga_StartsFromApi()
        {
            // Arrange
            await _apiFactory.SeedDatabase(DataSeeds);
            var addLead = new LeadApi.Client.Lead { Name = "Test Tester 3", ZipCode = "60642" };
            var session = _leadServicebusHostFactory.Services.GetService<IMessageSession>();

            // Act
            var result = await EndpointFixture.ExecuteAndWaitForSagaCompletion<LeadMatchingProcessSaga>(
                async () => await _leadClient.AddLeadFromMessagingAsync(addLead));

            // Assert Matching Process Finished
            var finalEvent = result.SentMessages.OfType<MatchingProcessFinished>()
                .FirstOrDefault();

            Assert.IsNotNull(finalEvent);
            Assert.IsTrue(finalEvent.MatchIDs.Count() == 4);

            // Assert Saga Data Is Correct
            var saga = result.InvokedHandlers.LastOrDefault(x =>
                x.MessageHandler.HandlerType == typeof(LeadMatchingProcessSaga)).GetSagaInstance();
            var sagaData = (LeadMatchingData)saga.Instance.Entity;

            Assert.IsNotNull(sagaData);
            Assert.IsTrue(sagaData.ContractorIDs.Count() == 4);
            Assert.AreEqual(addLead.Name, sagaData.Name);
            Assert.AreEqual(addLead.ZipCode, sagaData.ZipCode);

            // Assert Database is Up To Date
            var matches = await _leadClient.GetMatchesAsync(sagaData.LeadID);
            Assert.AreEqual(4, matches.Count());
        }

        [Test]
        public async Task Test_MatchingSagaNoMatches_StartsFromApi()
        {
            // Arrange
            await _apiFactory.SeedDatabase(DataSeeds);
            var addLead = new LeadApi.Client.Lead { Name = "Test Tester 3", ZipCode = "12345" };
            var session = _leadServicebusHostFactory.Services.GetService<IMessageSession>();

            // Act
            var result = await EndpointFixture.ExecuteAndWaitForSagaCompletion<LeadMatchingProcessSaga>(
                async () => await _leadClient.AddLeadFromMessagingAsync(addLead));

            // Assert Matching Process Finished
            var finalEvent = result.SentMessages.OfType<MatchingProcessFinished>()
                .FirstOrDefault();

            Assert.IsNotNull(finalEvent);
            Assert.IsTrue(finalEvent.MatchIDs.Count() == 0);

            // Assert Saga Data Is Correct
            var saga = result.InvokedHandlers.LastOrDefault(x =>
                x.MessageHandler.HandlerType == typeof(LeadMatchingProcessSaga)).GetSagaInstance();
            var sagaData = (LeadMatchingData)saga.Instance.Entity;

            Assert.IsNotNull(sagaData);
            Assert.IsTrue(sagaData.ContractorIDs.Count() == 0);
            Assert.AreEqual(addLead.Name, sagaData.Name);
            Assert.AreEqual(addLead.ZipCode, sagaData.ZipCode);

            // Assert Database is Up To Date
            var matches = await _leadClient.GetMatchesAsync(sagaData.LeadID);
            Assert.AreEqual(0, matches.Count());
        }
    }
}
