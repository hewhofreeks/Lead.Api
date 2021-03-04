using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fake.AvailabilityClient
{
    public interface IAvailabilityClient
    {
        Task<List<long>> SearchContractors(string zipCode);
    }

    public class AvailabilityClient : IAvailabilityClient
    {
        public Task<List<long>> SearchContractors(string zipCode)
        {
            Random rnd = new Random();
            int numContractors = rnd.Next(0, 4);

            List<long> contractorIDs = new List<long>();

            for (var i = 0; i < numContractors; i++)
            {
                contractorIDs.Add(rnd.Next(1, 8));
            }

            return Task.FromResult(contractorIDs);
        }
    }
}
