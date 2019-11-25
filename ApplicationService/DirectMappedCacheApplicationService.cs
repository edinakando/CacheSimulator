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

        public Address GetCurrentAddressBreakdown()
        {
            var addressInBits = GetCurrentAddressInBits();

            return new Address
            {
                TagBinary = _GetCurrentTag(),
                IndexBinary = addressInBits?.Substring(_GetTagSizeInBits(), _GetIndexSizeInBits()),
                OffsetBinary = _GetCurrentOffset()
            };
        }

        public CacheViewModel UpdateCache()
        {
            Int32 currentIndex = _GetCurrentIndex();
            Int32 address = SimulationParameters.Operations[CurrentOperationIndex].Address;
            Int32 cacheLineSizeInMemoryBlocks = SimulationParameters.DataSize / MemoryDataSize; //4
            Int32 blockIndex = address / cacheLineSizeInMemoryBlocks;

            CacheViewModel.Tags[currentIndex] = _GetCurrentTag();
            CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
            CacheViewModel.CurrentMemoryAddress = blockIndex;

            return CacheViewModel;
        }

        private String _GetCurrentTag()
        {
            return GetCurrentAddressInBits().Substring(0, _GetTagSizeInBits());
        }

        private Int32 _GetCurrentIndex()
        {
            var addressInBits = GetCurrentAddressInBits();  
            return Convert.ToInt32(addressInBits.Substring(_GetTagSizeInBits(), _GetIndexSizeInBits()), 2);
        }

        private String _GetCurrentOffset()
        {
            var tagSize = _GetTagSizeInBits();
            var indexSize = _GetIndexSizeInBits();

            return GetCurrentAddressInBits().Substring(tagSize + indexSize, GetOffsetSizeInBits());
        }
        private Int32 _GetIndexSizeInBits()
        {
            return Convert.ToInt32(Math.Log2(IndexCount));
        }

        private Int32 _GetTagSizeInBits()
        {
            return AddressSize - _GetIndexSizeInBits() - GetOffsetSizeInBits();
        }

        public WriteOperationViewModel WriteToMemory()
        {
            Int32 address = SimulationParameters.Operations[CurrentOperationIndex].Address;
            Int32 cacheLineSizeInMemoryBlocks = SimulationParameters.DataSize / MemoryDataSize; //4
            Int32 blockIndex = address / cacheLineSizeInMemoryBlocks;
            Int32 currentIndex = _GetCurrentIndex();
            String newData = SimulationParameters.Operations[CurrentOperationIndex].Data;
            var updatedData = new WriteOperationViewModel();

            if (SimulationParameters.WritePolicy == WritePolicy.WriteThrough)
            {
                if(SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteAllocate) //update cache and memory always
                {
                    Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
                    updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;

                    CacheViewModel.Tags[currentIndex] = _GetCurrentTag();
                    CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
                    CacheViewModel.CurrentMemoryAddress = blockIndex;
                    updatedData.IsCacheUpdated = true;
                    updatedData.IsMemoryUpdated = true;
                }
                else if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteNoAllocate)
                {

                }
            }

            updatedData.CacheViewModel = CacheViewModel;
            updatedData.Memory = Memory;
            return updatedData;
        }
    }
}
