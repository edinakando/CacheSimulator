using System;
using System.Collections.Generic;

namespace CacheSimulator.Models
{
    public class BlockModel
    {
        public Int32 BlockNumber { get; set; }
        public IList<String> Data { get; set; }
    }
}
