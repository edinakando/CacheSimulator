using CacheSimulator.Models;
using System;
using System.Collections.Generic;

namespace CacheSimulator.ViewModels
{
    public class CacheViewModel
    {
        public IList<String> Tags { get; set; }
        public IList<BlockModel> CacheLines { get; set; }
        public IList<Int32> DirtyBit { get; set; }
        public IList<Int32> UpdatedMemoryAddress { get; set; }
        public Int32 CurrentMemoryAddress { get; set; }
        public Int32 LastUpdatedIndex  { get; set; }
        public String CacheUpdateTypeMessage { get; set; }
        public CacheViewModel(Int32 size)
        {
            Tags = new List<String>(new String[size]);
            DirtyBit = new List<Int32>(new Int32[size]);
            CacheLines = new List<BlockModel>(new BlockModel[size]);
            UpdatedMemoryAddress = new List<Int32>(new Int32[size]);
        }
    }
}
