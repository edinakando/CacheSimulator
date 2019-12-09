using CacheSimulator.Models;
using CacheSimulator.ViewModels;
using System;
using System.Collections.Generic;

namespace CacheSimulator.ApplicationService
{
    public class SetAssociativeCacheApplicationService : CacheApplicationService
    {
        public static IDictionary<Int32, IList<Int32>> CacheLineFrequencies { get; set; } //pentru fiecare index - seturile
        public static IDictionary<Int32, IList<Int32>> CacheLineLastUsageTimes { get; set; }
        public static List<KeyValuePair<Int32, Int32>> Fifo { get; set; }
        public new static SetAssociativeCacheViewModel CacheViewModel { get; set; }

        public static void SetupSimulation(SimulationParameters simulationParameters)
        {
            SimulationParameters = simulationParameters;
            SetMemory();
            _SetIndexCount();
            CacheViewModel = new SetAssociativeCacheViewModel(simulationParameters.SetCount, IndexCount);

            CacheLineFrequencies = new Dictionary<Int32, IList<Int32>>();
            CacheLineLastUsageTimes = new Dictionary<Int32, IList<Int32>>();
           
            for(var set = 0; set < SimulationParameters.SetCount; set++)
            {
                CacheLineFrequencies.Add(set, new List<Int32>(new Int32[IndexCount]));
                CacheLineLastUsageTimes.Add(set, new List<Int32>(new Int32[IndexCount]));
            }
            Fifo = new List<KeyValuePair<Int32, Int32>>();
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

        public SetAssociativeCacheViewModel UpdateCache()
        {
            Boolean isFull = true;
            Int32 cacheLine = _GetCurrentIndex();  //asta e de fapt setul

            SetAssociativeCacheLineViewModel cacheLineToBeReplaced = GetCacheLineToBeReplaced();
           
            for (var set = 0; set < SimulationParameters.SetCount; set++)
            {
                if (CacheViewModel.Cache[set].CacheLines[cacheLine] == null)
                {
                    CacheViewModel.Cache[set].CacheUpdateTypeMessage = "Copying data to an unused cache line.";
                    isFull = false;
                    break;
                }
            }

            if (isFull && SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LFU)
            {
                CacheViewModel.Cache[cacheLine].CacheUpdateTypeMessage = "Copying data to least frequently used cache line.";
            }
            else if (isFull && SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LRU)
            {
                CacheViewModel.Cache[cacheLine].CacheUpdateTypeMessage = "Copying data to least recently used cache line.";
            }
            else if (isFull && SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.FIFO)
            {
                CacheViewModel.Cache[Fifo[0].Key].CacheUpdateTypeMessage = "Copying data to first cache line updated.";
                Fifo.RemoveAt(0);
            }

            _PlaceDataAtIndex(cacheLineToBeReplaced.Index, cacheLineToBeReplaced.Set);
            UpdateStatistics(cacheLineToBeReplaced.Set, cacheLineToBeReplaced.Index);
            CacheViewModel.LastUpdatedSet = cacheLineToBeReplaced.Set;

            return CacheViewModel;
        }

        private Int32 _GetCurrentIndex()
        {
            var addressInBits = GetCurrentAddressInBits();
            return Convert.ToInt32(addressInBits.Substring(_GetTagSizeInBits(), _GetIndexSizeInBits()), 2);
        }
        private String _GetCurrentTagInBits()
        {
            return GetCurrentAddressInBits().Substring(0, _GetTagSizeInBits());
        }

        private void _PlaceDataAtIndex(Int32 currentIndex, Int32 set)
        {
            Int32 address = SimulationParameters.Operations[CurrentOperationIndex].Address;
            Int32 cacheLineSizeInMemoryBlocks = SimulationParameters.DataSize / MemoryDataSize; //4
            Int32 blockIndex = address / cacheLineSizeInMemoryBlocks;

            CacheViewModel.Cache[set].Tags[currentIndex] = _GetCurrentTagInBits();
            CacheViewModel.Cache[set].CacheLines[currentIndex] = Memory[blockIndex];
            CacheViewModel.Cache[set].CurrentMemoryAddress = blockIndex;
            CacheViewModel.Cache[set].LastUpdatedIndex = currentIndex;
        }

        public void UpdateStatistics(Int32 currentIndex, Int32 currentSet = 0) //to be done
        {
            CacheLineFrequencies[currentSet][currentIndex]++;

            Int32 mostRecentTime = -1;
            IList<Int32> setUsageTimes = CacheLineLastUsageTimes[currentSet];
            for (var cacheLine = 0; cacheLine < setUsageTimes.Count; cacheLine++)
            {
                if (setUsageTimes[cacheLine] > mostRecentTime)
                {
                    mostRecentTime = setUsageTimes[cacheLine];
                }
            }
            CacheLineLastUsageTimes[currentSet][currentIndex] = ++mostRecentTime;
        }

        public WriteOperationViewModel WriteToMemory(Int32 currentIndex)
        {
            //Int32 address = SimulationParameters.Operations[CurrentOperationIndex].Address;
            //Int32 cacheLineSizeInMemoryBlocks = SimulationParameters.DataSize / MemoryDataSize; //4
            //Int32 blockIndex = address / cacheLineSizeInMemoryBlocks;
            //String newData = SimulationParameters.Operations[CurrentOperationIndex].Data;
            //var updatedData = new WriteOperationViewModel();

            //UpdateStatistics(currentIndex);

            //if (SimulationParameters.WritePolicy == WritePolicy.WriteThrough)
            //{
            //    if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteAllocate) //update cache and memory always
            //    {
            //        //miss => update block in memory and bring it to the cache
            //        if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTagInBits())
            //        {
            //            Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
            //            updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;

            //            CacheViewModel.Tags[currentIndex] = _GetCurrentTagInBits();
            //            CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
            //            CacheViewModel.CurrentMemoryAddress = blockIndex;

            //            updatedData.IsCacheUpdated = true;
            //            updatedData.IsMemoryUpdated = true;
            //        }
            //        else
            //        {
            //            //hit => write to cache and memory
            //            Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
            //            updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;

            //            CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
            //            CacheViewModel.CurrentMemoryAddress = blockIndex;

            //            updatedData.IsCacheUpdated = true;
            //            updatedData.IsMemoryUpdated = true;
            //        }
            //    }
            //    else if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteNoAllocate)
            //    {
            //        //miss => update block in memory, not bringing it to the cache
            //        if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTagInBits())
            //        {
            //            Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
            //            updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;
            //            updatedData.IsMemoryUpdated = true;
            //            CacheViewModel.CurrentMemoryAddress = blockIndex;
            //        }
            //        else
            //        {
            //            //hit => write to cache and memory
            //            Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
            //            updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;

            //            CacheViewModel.Tags[currentIndex] = _GetCurrentTagInBits();
            //            CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];
            //            CacheViewModel.CurrentMemoryAddress = blockIndex;

            //            updatedData.IsCacheUpdated = true;
            //            updatedData.IsMemoryUpdated = true;
            //        }
            //    }
            //}
            //else if (SimulationParameters.WritePolicy == WritePolicy.WriteBack)
            //{
            //    if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteAllocate)
            //    {
            //        //miss => update block in memory and bring it to cache
            //        if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTagInBits())
            //        {
            //            Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;

            //            CacheViewModel.Tags[currentIndex] = _GetCurrentTagInBits();
            //            CacheViewModel.CacheLines[currentIndex] = Memory[blockIndex];

            //            CacheViewModel.CurrentMemoryAddress = blockIndex;
            //            CacheViewModel.UpdatedMemoryAddress[currentIndex] = address;

            //            CacheViewModel.DirtyBit[currentIndex] = 0;

            //            updatedData.IsCacheUpdated = true;
            //            updatedData.IsMemoryUpdated = true;
            //            updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;
            //        }
            //        else //hit => write to cache, set dirty bit, NO write to memory
            //        {
            //            CacheViewModel.CacheLines[currentIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
            //            CacheViewModel.DirtyBit[currentIndex] = 1;
            //            CacheViewModel.UpdatedMemoryAddress[currentIndex] = address;
            //            updatedData.IsCacheUpdated = true;
            //        }
            //    }
            //    else if (SimulationParameters.WritePolicyAllocate == WritePolicyAllocate.WriteNoAllocate)
            //    {
            //        //miss => update memory but don't bring it to cache
            //        if (CacheViewModel.Tags == null || CacheViewModel.Tags[currentIndex] != _GetCurrentTagInBits())
            //        {
            //            Memory[blockIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
            //            CacheViewModel.CurrentMemoryAddress = blockIndex;
            //            updatedData.IsMemoryUpdated = true;
            //            updatedData.UpdatedPlaceInMemoryBlock = address % cacheLineSizeInMemoryBlocks;
            //        }
            //        else //hit => write to cache, set dirty bit, NO write to memory
            //        {
            //            CacheViewModel.CacheLines[currentIndex].Data[address % cacheLineSizeInMemoryBlocks] = newData;
            //            CacheViewModel.DirtyBit[currentIndex] = 1;
            //            CacheViewModel.UpdatedMemoryAddress[currentIndex] = address;
            //            updatedData.IsCacheUpdated = true;
            //        }
            //    }
            //}

            //updatedData.CacheViewModel = CacheViewModel;
            //updatedData.Memory = Memory;
            //return updatedData;
            return null;
        }

        public SetAssociativeCacheLineViewModel GetCacheLineToBeReplaced()
        {
            Int32 cacheLine = _GetCurrentIndex();  //asta e de fapt setul
            Boolean isFull = true;
            Int32 setCount = 0;
            Int32 indexCount = 0;

            for (var set = 0; set < SimulationParameters.SetCount; set++)
            {
                if (CacheViewModel.Cache[set].CacheLines[cacheLine] == null)
                {
                    CacheViewModel.LastUpdatedSet = set;
                    Fifo.Add(new KeyValuePair<int, int>(cacheLine, set));
                    isFull = false;
                    setCount = set;
                    indexCount = cacheLine;
                    break;
                }
            }

            if (isFull && SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LFU)
            {
                IList<Int32> setFrequencies = CacheLineFrequencies[cacheLine];
                Int32 frequency = setFrequencies[0];
                Int32 replacedIndexFromSet = 0;

                for (var index = 1; index < setFrequencies.Count; index++)
                {
                    if (setFrequencies[index] < frequency)
                    {
                        replacedIndexFromSet = index;
                        frequency = setFrequencies[index];
                    }
                }

                setCount = replacedIndexFromSet;
                indexCount = cacheLine;
            }
            else if (SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LRU)
            {
                IList<Int32> setUsageTimes = CacheLineLastUsageTimes[cacheLine];
                Int32 usageTime = setUsageTimes[0];
                Int32 replacedIndexFromSet = 0;

                for (var index = 1; index < setUsageTimes.Count; index++)
                {
                    if (setUsageTimes[index] < usageTime)
                    {
                        replacedIndexFromSet = index;
                        usageTime = setUsageTimes[index];
                    }
                }

                setCount = replacedIndexFromSet;
                indexCount = cacheLine;
            }
            else if (SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.FIFO)
            {
                setCount = Fifo[0].Value;
                indexCount = Fifo[0].Key;
            }

            return new SetAssociativeCacheLineViewModel
            {
                Set = setCount,
                Index = indexCount
            };
        }

        public WriteOperationViewModel UpdateMemory(Int32 currentIndex)
        {
        //Int32 updatedMemoryBlock = CacheViewModel.UpdatedMemoryAddress[currentIndex];
        //Memory[updatedMemoryBlock] = CacheViewModel.CacheLines[currentIndex];

        //Int32 cacheLineSizeInMemoryBlocks = SimulationParameters.DataSize / MemoryDataSize; //4
        //CacheViewModel.CurrentMemoryAddress = CacheViewModel.UpdatedMemoryAddress[currentIndex] / cacheLineSizeInMemoryBlocks;
        //CacheViewModel.DirtyBit[currentIndex] = 0;

        //var updatedData = new WriteOperationViewModel();
        //updatedData.Memory = Memory;
        //updatedData.CacheViewModel = CacheViewModel;

        //updatedData.IsMemoryUpdated = true;

        //return updatedData;
        return null;
        }
    }

   
}
