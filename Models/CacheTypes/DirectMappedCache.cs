using System;
using System.Collections.Generic;

namespace CacheSimulator.Models.CacheTypes
{
    public class DirectMappedCache
    {
        public Int32 Size { get; set; }
        public IList<Int32> Valid { get; set; }
        public IList<Int32> Tag { get; set; }
        public IList<String> Data { get; set; }
    }
}
