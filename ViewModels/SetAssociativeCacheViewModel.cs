using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CacheSimulator.ViewModels
{
    public class SetAssociativeCacheViewModel
    {
        public IList<CacheViewModel> Cache { get; set; }
        public Int32 LastUpdatedSet { get; set; }


        public SetAssociativeCacheViewModel(Int32 setCount, Int32 indexCount)
        {
            Cache = new List<CacheViewModel>();

            for (var set = 0; set < setCount; set++)
            {
                Cache.Add(new CacheViewModel(indexCount));
            }

        }
    }
}
