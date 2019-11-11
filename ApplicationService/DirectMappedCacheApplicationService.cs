using CacheSimulator.Models;
using CacheSimulator.ViewModels;
using System;

namespace CacheSimulator.ApplicationService
{
    public class DirectMappedCacheApplicationService : CacheApplicationService
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

        public static void Reset()
        {
            SimulationParameters = null;
            CurrentOperationIndex = 0;
        }

        public Address GetCurrentAddressBreakdown()
        {
            var addressInBits = GetCurrentAddressInBits();
            var tagSize = _GetTagSizeInBits();

            return new Address
            {
                TagBinary = addressInBits?.Substring(0, tagSize),
                IndexBinary = addressInBits?.Substring(tagSize, _GetIndexSizeInBits())
            };
        }

        public CacheViewModel UpdateCache()
        {
            Int32 currentIndex = _GetCurrentIndex();
            Int32 address = SimulationParameters.Operations[CurrentOperationIndex].Address;

            CacheViewModel.Tags[currentIndex] = _GetCurrentTag();
            CacheViewModel.DataBlocks[currentIndex] = Memory[address];
            CacheViewModel.CurrentMemoryAddress = address;

            return CacheViewModel;
        }

        public void NextInstruction()
        {
            CurrentOperationIndex++;
        }

        private Int32 _GetCurrentIndex()
        {
            var addressInBits = GetCurrentAddressInBits();  
            return Convert.ToInt32(addressInBits.Substring(_GetTagSizeInBits(), _GetIndexSizeInBits()), 2);
        }

        private String _GetCurrentTag()
        {
            return GetCurrentAddressInBits().Substring(0, _GetTagSizeInBits());
        }

        private Int32 _GetIndexSizeInBits()
        {
            return Convert.ToInt32(Math.Log2(IndexCount));
        }

        private Int32 _GetTagSizeInBits()
        {
            return AddressSize - _GetIndexSizeInBits();
        }
    }
}
