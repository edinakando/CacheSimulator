using CacheSimulator.Models;
using CacheSimulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CacheSimulator.ApplicationService
{
    public class FullyAssociativeCacheApplicationService : CacheApplicationService
    {
        public static void SetupSimulation(SimulationParameters simulationParameters)
        {
            SimulationParameters = simulationParameters;
            SetMemory();
            _SetIndexCount();
            CacheViewModel = new CacheViewModel(IndexCount);
        }

        private static void _SetIndexCount()
        {
            IndexCount = SimulationParameters.CacheSize / SimulationParameters.DataSize;
        }

        public Address GetCurrentAddressBreakdown()
        {
            var addressInBits = GetCurrentAddressInBits();

            return new Address
            {
                TagBinary = addressInBits?.Substring(0, AddressSize),
            };
        }

    }
}
