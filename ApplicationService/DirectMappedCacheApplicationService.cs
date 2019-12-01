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

            //check dirty bit => maybe on front-end => call a different method + another step
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

        public WriteOperationViewModel UpdateMemoryOnRead()
        {
            Int32 currentIndex = _GetCurrentIndex();
            Int32 updatedMemoryBlock = CacheViewModel.UpdatedMemoryAddress[currentIndex];
            Memory[updatedMemoryBlock] = CacheViewModel.CacheLines[currentIndex];

            Int32 cacheLineSizeInMemoryBlocks = SimulationParameters.DataSize / MemoryDataSize; //4
            CacheViewModel.CurrentMemoryAddress = CacheViewModel.UpdatedMemoryAddress[currentIndex] / cacheLineSizeInMemoryBlocks;
            CacheViewModel.DirtyBit[currentIndex] = 0;

            var updatedData = new WriteOperationViewModel();
            updatedData.Memory = Memory;
            updatedData.CacheViewModel = CacheViewModel;

            updatedData.IsMemoryUpdated = true;

            return updatedData;
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
                if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteAllocate) //update cache and memory always
                {
                    //miss => update block in memory and bring it to the cache
                    if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTag())
                    {
                        Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
                        updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;

                        CacheViewModel.Tags[currentIndex] = _GetCurrentTag();
                        CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
                        CacheViewModel.CurrentMemoryAddress = blockIndex;

                        updatedData.IsCacheUpdated = true;
                        updatedData.IsMemoryUpdated = true;
                    }
                    else
                    {
                        //hit => write to cache and memory
                        Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
                        updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;

                        CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
                        CacheViewModel.CurrentMemoryAddress = blockIndex;

                        updatedData.IsCacheUpdated = true;
                        updatedData.IsMemoryUpdated = true;
                    }
                }
                else if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteNoAllocate)
                {
                    //miss => update block in memory, not bringing it to the cache
                    if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTag())
                    {
                        Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
                        updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;
                        updatedData.IsMemoryUpdated = true;
                        CacheViewModel.CurrentMemoryAddress = blockIndex;
                    }
                    else
                    {
                        //hit => write to cache and memory
                        Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
                        updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;

                        CacheViewModel.Tags[currentIndex] = _GetCurrentTag();
                        CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
                        CacheViewModel.CurrentMemoryAddress = blockIndex;

                        updatedData.IsCacheUpdated = true;
                        updatedData.IsMemoryUpdated = true;
                    }
                }
            }
            else if (SimulationParameters.WritePolicy == WritePolicy.WriteBack)
            {
                if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteAllocate)
                {
                    //miss => update block in memory and bring it to cache
                    if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTag())
                    {
                        Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;

                        CacheViewModel.Tags[currentIndex] = _GetCurrentTag();
                        CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];

                        CacheViewModel.CurrentMemoryAddress = blockIndex;
                        CacheViewModel.UpdatedMemoryAddress[currentIndex] = address;

                        CacheViewModel.DirtyBit[currentIndex] = 0;

                        updatedData.IsCacheUpdated = true;
                        updatedData.IsMemoryUpdated = true;
                        updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;
                    }
                    else //hit => write to cache, set dirty bit, NO write to memory
                    {
                        CacheViewModel.CacheLines[currentIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
                        CacheViewModel.DirtyBit[currentIndex] = 1;
                        CacheViewModel.UpdatedMemoryAddress[currentIndex] = address;
                        updatedData.IsCacheUpdated = true;
                    }
                }
                else if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteNoAllocate)
                {
                    //miss => update memory but don't bring it to cache
                    if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTag())
                    {
                        Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
                        CacheViewModel.CurrentMemoryAddress = blockIndex;
                        updatedData.IsMemoryUpdated = true;
                        updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;
                    }
                    else //hit => write to cache, set dirty bit, NO write to memory
                    {
                        CacheViewModel.CacheLines[currentIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
                        CacheViewModel.DirtyBit[currentIndex] = 1;
                        CacheViewModel.UpdatedMemoryAddress[currentIndex] = address;
                        updatedData.IsCacheUpdated = true;
                    }
                }
            }

            updatedData.CacheViewModel = CacheViewModel;
            updatedData.Memory = Memory;
            return updatedData;
        }
    }


}
