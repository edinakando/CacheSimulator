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
        protected static IList<BlockModel> Memory;
        protected static Int32 CurrentOperationIndex = 0;
        protected static Int32 IndexCount;
        protected static Int32 AddressSize = 12;
        protected static Int32 MemoryDataSize = 8;

        protected static void SetMemory()
        {
            Memory = new List<BlockModel>();
            Int32 offsetSize = SimulationParameters.CacheSize / SimulationParameters.DataSize;
            Int32 blockNumbers = SimulationParameters.MemorySize / offsetSize;
            Int32 dataBlockNumber = 0;
            for (var blockNumber = 0; blockNumber < blockNumbers; blockNumber++)
            {
                var currentBlockList = new List<String>();
                for(var dataBlock = 0; dataBlock < offsetSize; dataBlock++)
                {
                    currentBlockList.Add("D" + dataBlockNumber);
                    dataBlockNumber++;
                }
                Memory.Add(new BlockModel
                {
                    BlockNumber = blockNumber, 
                    Data = currentBlockList 
                });
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
        protected Int32 GetOffsetSizeInBits()
        {
            return Convert.ToInt32(Math.Log2(SimulationParameters.DataSize / MemoryDataSize));
        }

        public void NextInstruction()
        {
            CurrentOperationIndex++;
        }

        public static void Reset()
        {
            SimulationParameters = null;
            CurrentOperationIndex = 0;
        }
    }
}
