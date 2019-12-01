using CacheSimulator.Models;
using CacheSimulator.ViewModels;
using System;
using System.Collections.Generic;

namespace CacheSimulator.ApplicationService
{
    public class FullyAssociativeCacheApplicationService : CacheApplicationService
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
            IndexCount = SimulationParameters.CacheSize / SimulationParameters.DataSize;
        }

        public Address GetCurrentAddressBreakdown()
        {
            String addressInBits = GetCurrentAddressInBits();

            return new Address
            {
                TagBinary = addressInBits.Substring(0, _GetTagSizeInBits()),
                OffsetBinary = addressInBits.Substring(_GetTagSizeInBits(), GetOffsetSizeInBits()),
                IndexBinary = Convert.ToString(GetIndexCount(), 2)
            };
        }

        private Int32 _GetTagSizeInBits()
        {
            return AddressSize - GetOffsetSizeInBits();
        }
        private String _GetCurrentTagInBits()
        {
            return GetCurrentAddressInBits().Substring(0, _GetTagSizeInBits());
        }

        public CacheViewModel UpdateCache()
        {
            Int32 cacheIndexToBeReplaced = GetIndexToBeReplaced();
            Boolean isFull = true;

            for(var cacheLine = 0; cacheLine < IndexCount; cacheLine++)
            {
                if(CacheViewModel.CacheLines[cacheLine] == null)
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull)
            {
                if (SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LFU)
                {
                    CacheViewModel.CacheUpdateTypeMessage = "Copying data to least frequently used cache line.";
                }
                else if (SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LRU)
                {
                    CacheViewModel.CacheUpdateTypeMessage = "Copying data to least recently used cache line.";
                }
                else if (SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.FIFO)
                {
                    Fifo.RemoveAt(0);
                    CacheViewModel.CacheUpdateTypeMessage = "Copying data to first cache line updated.";
                }
            }
            else
            {
                Fifo.Add(cacheIndexToBeReplaced);
                CacheViewModel.CacheUpdateTypeMessage = "Copying data to an unused cache line.";
            }

            _PlaceDataAtIndex(cacheIndexToBeReplaced);
            return CacheViewModel;
        }

        private void _PlaceDataAtIndex(Int32 currentIndex)
        {
            if(CacheViewModel.DirtyBit[currentIndex] == 1)
            {
                WriteToMemory(currentIndex);
            }

            Int32 address = SimulationParameters.Operations[CurrentOperationIndex].Address;
            Int32 cacheLineSizeInMemoryBlocks = SimulationParameters.DataSize / MemoryDataSize; //4
            Int32 blockIndex = address / cacheLineSizeInMemoryBlocks;

            CacheViewModel.Tags[currentIndex] = _GetCurrentTagInBits();
            CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
            CacheViewModel.CurrentMemoryAddress = blockIndex;
            CacheViewModel.LastUpdatedIndex = currentIndex;

            UpdateStatistics(currentIndex);
        }

        public void UpdateStatistics(Int32 currentIndex)
        {
            CacheLineFrequencies[currentIndex]++;
            
            Int32 mostRecentTime = -1;
            for(var cacheLine = 0; cacheLine < IndexCount; cacheLine++)
            {
                if(CacheLineLastUsageTimes[cacheLine] > mostRecentTime)
                {
                    mostRecentTime = CacheLineLastUsageTimes[cacheLine];
                }
            }
            CacheLineLastUsageTimes[currentIndex] = ++mostRecentTime;

            Fifo.Add(currentIndex);  //?is dis right?
        }

        public WriteOperationViewModel WriteToMemory(Int32 currentIndex)
        {
            Int32 address = SimulationParameters.Operations[CurrentOperationIndex].Address;
            Int32 cacheLineSizeInMemoryBlocks = SimulationParameters.DataSize / MemoryDataSize; //4
            Int32 blockIndex = address / cacheLineSizeInMemoryBlocks;
            String newData = SimulationParameters.Operations[CurrentOperationIndex].Data;
            var updatedData = new WriteOperationViewModel();
            UpdateStatistics(currentIndex);

            if (SimulationParameters.WritePolicy == WritePolicy.WriteThrough)
            {
                if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteAllocate) //update cache and memory always
                {
                    //miss => update block in memory and bring it to the cache
                    if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTagInBits())
                    {
                        Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
                        updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;

                        CacheViewModel.Tags[currentIndex] = _GetCurrentTagInBits();
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
                    if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTagInBits())
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

                        CacheViewModel.Tags[currentIndex] = _GetCurrentTagInBits();
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
                    if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTagInBits())
                    {
                        Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;

                        CacheViewModel.Tags[currentIndex] = _GetCurrentTagInBits();
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
                    if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTagInBits())
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

        public Int32 GetIndexToBeReplaced()
        {
            for (var cacheLine = 0; cacheLine < IndexCount; cacheLine++)
            {
                if (CacheViewModel.CacheLines[cacheLine] == null)
                {
                    return cacheLine;
                }
            }

            if (SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LFU)
            {
                Int32 frequency = CacheLineFrequencies[0];
                Int32 replacedIndex = 0;

                for (var index = 1; index < CacheLineFrequencies.Count; index++)
                {
                    if (CacheLineFrequencies[index] < frequency)
                    {
                        replacedIndex = index;
                        frequency = CacheLineFrequencies[index];
                    }
                }

                return replacedIndex;
            }
           
            if (SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LRU)
            {
                Int32 usageTime = CacheLineLastUsageTimes[0];
                Int32 replacedIndex = 0;

                for (var index = 1; index < CacheLineLastUsageTimes.Count; index++)
                {
                    if (CacheLineLastUsageTimes[index] < usageTime)
                    {
                        replacedIndex = index;
                        usageTime = CacheLineLastUsageTimes[index];
                    }
                }

                return replacedIndex;
            }
            
            if (SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.FIFO)
            {
                return Fifo[0];
            }

            return 0;
        }

        public WriteOperationViewModel UpdateMemory(Int32 currentIndex)
        {
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
    }

}
