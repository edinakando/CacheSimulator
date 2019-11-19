using CacheSimulator.ApplicationService;
using CacheSimulator.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace CacheSimulator.Controllers
{
    public class DirectMappedCacheSimulationController : Controller
    {

        private readonly DirectMappedCacheApplicationService _applicationService;

        public DirectMappedCacheSimulationController(DirectMappedCacheApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void DirectMappedCache(SimulationParameters simulationParameters)
        {
            DirectMappedCacheApplicationService.SetupSimulation(simulationParameters);
        }

        [HttpGet]
        public IActionResult GetCurrentAddressBreakdown()
        {
            return Json( _applicationService.GetCurrentAddressBreakdown());
        }
      
        [HttpGet]
        public IActionResult UpdateCache()
        {
            return Json(_applicationService.UpdateCache());
        }

        public void NextInstruction()
        {
            _applicationService.NextInstruction();
        }

        public void Reset()
        {
            DirectMappedCacheApplicationService.Reset();
        }
    }
}