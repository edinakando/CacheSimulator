using CacheSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CacheSimulator.ViewModels
{
    public class CacheViewModel
    {
        public IList<String> Tags { get; set; }
        public IList<BlockModel> CacheLines { get; set; }
        public Int32 CurrentMemoryAddress { get; set; }
        public Int32 LastUpdatedIndex  { get; set; }
        public String CacheUpdateTypeMessage { get; set; }
        public CacheViewModel(Int32 size)
        {
            Tags = new List<String>(new String[size]);
            CacheLines = new List<BlockModel>(new BlockModel[size]);
        }
    }
}
