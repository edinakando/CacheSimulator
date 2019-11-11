using CacheSimulator.Models.CacheTypes;
using System;
using System.Collections.Generic;

namespace CacheSimulator.Models
{
    public class SimulationParameters
    {
        public IList<Operation> Operations { get; set; }
        public Int32 CacheSize { get; set; }
        public Int32 MemorySize { get; set; }
        public Int32 DataSize { get; set; }
        public ReplacementAlgorithm ReplacementAlgorithm { get; set; }
        public WritePolicy WritePolicy { get; set; }
        public WritePolicyAllocate WritePolicyAllocate  { get; set; }
    }

    public enum ReplacementAlgorithm
    {
        LFU,
        LRU,
        FIFO
    }

    public enum WritePolicy
    {
        WriteBack,
        WriteThrough
    }

    public enum WritePolicyAllocate
    {
        WriteAllocate,
        WriteNoAllocate
    }

}
