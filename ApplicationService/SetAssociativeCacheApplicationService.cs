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
        public new static SetAssociativeCacheViewModel CacheViewModel { get; set; }
        public static void SetupSimulation(SimulationParameters simulationParameters)
        {
            SimulationParameters = simulationParameters;
            SetMemory();
            _SetIndexCount();
            CacheViewModel = new SetAssociativeCacheViewModel(simulationParameters.SetCount, IndexCount);

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

        public SetAssociativeCacheViewModel UpdateCache()
        {
            Boolean isFull = true;
            Int32 cacheLine = _GetCurrentIndex();
            for (var set = 0; set < SimulationParameters.SetCount; set++)
            {
                if (CacheViewModel.Cache[set].CacheLines[cacheLine] == null)
                {
                    _PlaceDataAtIndex(cacheLine, set);
                    Fifo.Add(cacheLine);
                    CacheViewModel.Cache[set].CacheUpdateTypeMessage = "Copying data to an unused cache line.";
                    isFull = false;
                    break;
                }
            }

            //if (isFull && SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LFU)
            //{
            //    Int32 frequency = CacheLineFrequencies[0];
            //    Int32 replacedIndex = 0;

            //    for (var index = 1; index < CacheLineFrequencies.Count; index++)
            //    {
            //        if (CacheLineFrequencies[index] < frequency)
            //        {
            //            replacedIndex = index;
            //            frequency = CacheLineFrequencies[index];
            //        }
            //    }

            //    _PlaceDataAtIndex(replacedIndex);
            //    CacheViewModel.CacheUpdateTypeMessage = "Copying data to least frequently used cache line.";
            //}
            //else if (isFull && SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LRU)
            //{
            //    Int32 usageTime = CacheLineLastUsageTimes[0];
            //    Int32 replacedIndex = 0;

            //    for (var index = 1; index < CacheLineLastUsageTimes.Count; index++)
            //    {
            //        if (CacheLineLastUsageTimes[index] < usageTime)
            //        {
            //            replacedIndex = index;
            //            usageTime = CacheLineLastUsageTimes[index];
            //        }
            //    }

            //    _PlaceDataAtIndex(replacedIndex);
            //    CacheViewModel.CacheUpdateTypeMessage = "Copying data to least recently used cache line.";
            //}
            //else if (isFull && SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.FIFO)
            //{
            //    _PlaceDataAtIndex(Fifo[0]);
            //    Fifo.RemoveAt(0);
            //    CacheViewModel.CacheUpdateTypeMessage = "Copying data to first cache line updated.";
            //}

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
            CacheViewModel.LastUpdatedSet = set;

            UpdateStatistics(currentIndex);  //poate si pe seturi?

        }

        public void UpdateStatistics(Int32 currentIndex)
        {
            CacheLineFrequencies[currentIndex]++;

            Int32 mostRecentTime = -1;
            for (var cacheLine = 0; cacheLine < IndexCount; cacheLine++)
            {
                if (CacheLineLastUsageTimes[cacheLine] > mostRecentTime)
                {
                    mostRecentTime = CacheLineLastUsageTimes[cacheLine];
                }
            }
            CacheLineLastUsageTimes[currentIndex] = ++mostRecentTime;
        }
    }

   
}
