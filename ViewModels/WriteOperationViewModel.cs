using CacheSimulator.Models;
using System;
using System.Collections.Generic;

namespace CacheSimulator.ViewModels
{
    public class WriteOperationViewModel
    {
        public CacheViewModel  CacheViewModel { get; set; }
        public IList<BlockModel> Memory { get; set; }

        public Boolean IsCacheUpdated { get; set; } = false;
        public Boolean IsMemoryUpdated { get; set; } = false;
        public Boolean AreTagsEqual { get; set; } = false;

        public Int32 UpdatedPlaceInMemoryBlock { get; set; }

    }
}
