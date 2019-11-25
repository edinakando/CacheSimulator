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
                    Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
                    updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;
                    updatedData.IsMemoryUpdated = true;
                    CacheViewModel.CurrentMemoryAddress = blockIndex;
                }
            }
            else if(SimulationParameters.WritePolicy == WritePolicy.WriteBack)
            {
                if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteAllocate)
                {
                    if(CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTag()) //cache miss
                    {
                        var updatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;

                        CacheViewModel.Tags[currentIndex] = _GetCurrentTag();

                        CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
                        CacheViewModel.CacheLines[currentIndex].Data[updatedPlaceInMemoryBlock] = newData;

                        CacheViewModel.CurrentMemoryAddress = blockIndex;
                        CacheViewModel.UpdatedMemoryAddress[currentIndex] = address;

                        CacheViewModel.DirtyBit[currentIndex] = 1;

                        updatedData.IsCacheUpdated = true;
                    }
                    else //cache hit
                    {
                        if(CacheViewModel.DirtyBit[currentIndex] == 1)
                        {
                            //copy data from cache to memory
                            var updatedMemoryBlockIndex = CacheViewModel.UpdatedMemoryAddress[currentIndex] / cacheLineSizeInMemoryBlocks;
                            Memory[updatedMemoryBlockIndex].Data = CacheViewModel.CacheLines[currentIndex].Data; //copiaza tot blocul
                            updatedData.UpdatedPlaceInMemoryBlock = CacheViewModel.UpdatedMemoryAddress[currentIndex] % cacheLineSizeInMemoryBlocks;

                            CacheViewModel.Tags[currentIndex] = _GetCurrentTag();

                            CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
                            CacheViewModel.CacheLines[currentIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;

                            CacheViewModel.CurrentMemoryAddress = blockIndex;
                            CacheViewModel.UpdatedMemoryAddress[currentIndex] = address;

                            updatedData.IsMemoryUpdated = true;
                            updatedData.IsCacheUpdated = true;
                        }
                        else
                        {
                            CacheViewModel.Tags[currentIndex] = _GetCurrentTag();
                            CacheViewModel.DirtyBit[currentIndex] = 1;
                            CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
                            CacheViewModel.CurrentMemoryAddress = blockIndex;
                            updatedData.IsCacheUpdated = true;
                        }
                    }
                    
                }
            }

            updatedData.CacheViewModel = CacheViewModel;
            updatedData.Memory = Memory;
            return updatedData;
        }
    }
}
