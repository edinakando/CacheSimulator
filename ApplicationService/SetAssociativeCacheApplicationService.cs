using CacheSimulator.Models;
using CacheSimulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CacheSimulator.ApplicationService
{
    public class SetAssociativeCacheApplicationService : CacheApplicationService
    {
        public static IList<Int32> CacheLineFrequencies { get; set; }
        public static IList<Int32> CacheLineLastUsageTimes { get; set; }
        public static IList<Int32> Fifo { get; set; }

        public static void SetupSimulation(SimulationParameters simulationParameters)
        {
            SimulationParameters = simulationParameters;
            SetMemory();
            //_SetIndexCount();
            CacheViewModel = new CacheViewModel(IndexCount);

            CacheLineFrequencies = new List<Int32>(new Int32[IndexCount]);
            CacheLineLastUsageTimes = new List<Int32>(new Int32[IndexCount]);
            Fifo = new List<Int32>();
        }

        private void _SetIndexCount()
        {
            //IndexCount = SimulationParameters
        }

    }
}
