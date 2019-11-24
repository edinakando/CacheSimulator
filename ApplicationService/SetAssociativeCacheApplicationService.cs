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
            _SetIndexCount();
            CacheViewModel = new CacheViewModel(IndexCount);

            CacheLineFrequencies = new List<Int32>(new Int32[IndexCount]);
            CacheLineLastUsageTimes = new List<Int32>(new Int32[IndexCount]);
            Fifo = new List<Int32>();
        }

        private static void _SetIndexCount()
        {
            Int32 totalCacheLinesCount = SimulationParameters.CacheSize / SimulationParameters.DataSize;
            IndexCount = totalCacheLinesCount / SimulationParameters.SetCount;
        }

        public Address GetCurrentAddressBreakdown()
        {
            String addressInBits = GetCurrentAddressInBits();
            Int32 tagSize = _GetTagSizeInBits();
            Int32 indexSize = _GetIndexSizeInBits();

            return new Address
            {
                TagBinary = addressInBits.Substring(0, tagSize),
                IndexBinary = addressInBits.Substring(tagSize, indexSize),
                OffsetBinary = addressInBits.Substring(tagSize + indexSize , GetOffsetSizeInBits()),
            };
        }

        private Int32 _GetIndexSizeInBits()
        {
            return Convert.ToInt32(Math.Log2(IndexCount));
        }

        private Int32 _GetTagSizeInBits()
        {
            return AddressSize - _GetIndexSizeInBits() - GetOffsetSizeInBits();
        }
    }
}
