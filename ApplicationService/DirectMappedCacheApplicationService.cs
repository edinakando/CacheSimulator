using CacheSimulator.Models;
using CacheSimulator.ViewModels;
using System;
using System.Collections.Generic;

namespace CacheSimulator.ApplicationService
{
    public class DirectMappedCacheApplicationService
    {
        private static Int32 _addressSize = 12;
        private static Int32 _indexCount;
        private static Int32 _indexSize = 0;
        private static Int32 _tagSize = 0;
        private static Int32 _currentOperationIndex = 0;
        private static SimulationParameters _simulationParameters;
        private static CacheViewModel _cacheViewModel;
        private static IList<String> _memory;

        public static void SetupSimulation(SimulationParameters simulationParameters)
        {
            _simulationParameters = simulationParameters;
            _SetIndexCount();
            _SetAddressInfo();
            _SetMemory();

            _cacheViewModel = new CacheViewModel(_indexCount);
        }
        private static void _SetIndexCount()
        {
            _indexCount = _simulationParameters.CacheSize / _simulationParameters.DataSize;
        }

        private static void _SetAddressInfo()
        {
            _indexSize = Convert.ToInt32(Math.Log2(_indexCount));
            _tagSize = _addressSize - _indexSize;
        }

        public static void Reset()
        {
            _simulationParameters = null;
            _currentOperationIndex = 0;
        }

        private static void _SetMemory()
        {
            _memory = new List<String>();
            for(var i = 0; i < _simulationParameters.MemorySize; i++)
            {
                _memory.Add("B " + i);
            }
        }

        public Address GetCurrentAddressBreakdown()
        {
            Operation currentOperation = _simulationParameters.Operations[_currentOperationIndex++];  

            if(currentOperation == null)
            {
                return null;
            }

            var currentOperationAddress = currentOperation.Address;
            var addressInBits = Convert.ToString(currentOperationAddress, 2).PadLeft(12, '0');  //convert address to binary

            return new Address
            {
                TagBinary = addressInBits.Substring(0, _tagSize),
                IndexBinary = addressInBits.Substring(_tagSize, _indexSize)
            };
        }

        public CacheViewModel UpdateCache()
        {
            Int32 currentIndex = _GetCurrentIndex();
            Int32 address = _simulationParameters.Operations[_currentOperationIndex - 1].Address;

            _cacheViewModel.Tags[currentIndex] = _GetCurrentTag();
            _cacheViewModel.DataBlocks[currentIndex] = _memory[address];
            _cacheViewModel.CurrentMemoryAddress = address;

            return _cacheViewModel;
        }

        private Int32 _GetCurrentIndex()
        {
            var currentOperationAddress = _simulationParameters.Operations[_currentOperationIndex - 1].Address;
            var addressInBits = Convert.ToString(currentOperationAddress, 2).PadLeft(12, '0');  

            return Convert.ToInt32(addressInBits.Substring(_tagSize, _indexSize), 2);
        }

        private String _GetCurrentTag()
        {
            var currentOperationAddress = _simulationParameters.Operations[_currentOperationIndex - 1].Address;
            var addressInBits = Convert.ToString(currentOperationAddress, 2).PadLeft(12, '0');

            return addressInBits.Substring(0, _tagSize);
        }
    }
}
