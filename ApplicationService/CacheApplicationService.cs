using CacheSimulator.Models;
using CacheSimulator.ViewModels;
using System;
using System.Collections.Generic;

namespace CacheSimulator.ApplicationService
{
    public class CacheApplicationService
    {
        protected static SimulationParameters SimulationParameters;
        protected static CacheViewModel CacheViewModel;
        protected static IList<String> Memory;
        protected static Int32 CurrentOperationIndex = 0;
        protected static Int32 IndexCount;
        protected static Int32 AddressSize = 12;

        protected static void SetMemory()
        {
            Memory = new List<String>();
            for (var i = 0; i < SimulationParameters.MemorySize; i++)
            {
                Memory.Add("B " + i);
            }
        }

        protected String GetCurrentAddressInBits()
        {
            Operation currentOperation = SimulationParameters.Operations[CurrentOperationIndex];

            if (currentOperation == null)
            {
                return null;
            }
           
            return Convert.ToString(currentOperation.Address, 2).PadLeft(AddressSize, '0');
        }
    }
}
