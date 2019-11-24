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
            Boolean isFull = true;

            for(var cacheLine = 0; cacheLine < IndexCount; cacheLine++)
            {
                if(CacheViewModel.CacheLines[cacheLine] == null)
                {
                    _PlaceDataAtIndex(cacheLine);
                    Fifo.Add(cacheLine);
                    CacheViewModel.CacheUpdateTypeMessage = "Copying data to an unused cache line.";
                    isFull = false;
                    break;
                }
            }

            if(isFull && SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LFU)
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
               
                _PlaceDataAtIndex(replacedIndex);
                CacheViewModel.CacheUpdateTypeMessage = "Copying data to least frequently used cache line.";
            }
            else if(isFull && SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.LRU)
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

                _PlaceDataAtIndex(replacedIndex);
                CacheViewModel.CacheUpdateTypeMessage = "Copying data to least recently used cache line.";
            }
            else if (isFull && SimulationParameters.ReplacementAlgorithm == ReplacementAlgorithm.FIFO)
            {
                _PlaceDataAtIndex(Fifo[0]);
                Fifo.RemoveAt(0);
                CacheViewModel.CacheUpdateTypeMessage = "Copying data to first cache line updated.";
            }

            return CacheViewModel;
        }

        private void _PlaceDataAtIndex(Int32 currentIndex)
        {
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
        }
    }
}
