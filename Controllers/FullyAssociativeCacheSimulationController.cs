using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CacheSimulator.ApplicationService;
using CacheSimulator.Models;
using Microsoft.AspNetCore.Mvc;

namespace CacheSimulator.Controllers
{
    public class FullyAssociativeCacheSimulationController : Controller
    {
        private readonly FullyAssociativeCacheApplicationService _applicationService;

        public FullyAssociativeCacheSimulationController(FullyAssociativeCacheApplicationService applicationService)
        {
            _applicationService = applicationService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FullyAssociativeCache(SimulationParameters simulationParameters)
        {
            FullyAssociativeCacheApplicationService.SetupSimulation(simulationParameters);
            return Json(_applicationService.GetIndexCount());
        }

        public IActionResult GetCurrentAddressBreakdown()
        {
            return Json(_applicationService.GetCurrentAddressBreakdown());
        }

        [HttpGet]
        public IActionResult UpdateCache()
        {
            return Json(_applicationService.UpdateCache());
        }

        public void Reset()
        {
            FullyAssociativeCacheApplicationService.Reset();
        }

        public void NextInstruction()
        {
            _applicationService.NextInstruction();
        }

        public void CacheHit(Int32 index)
        {
            _applicationService.UpdateStatistics(index);
        }

        [HttpGet]
        public IActionResult WriteToMemory(Int32 index)
        {
            return Json(_applicationService.WriteToMemory(index));
        }

        [HttpGet]
        public IActionResult GetIndexToBeReplaced()
        {
            return Json(_applicationService.GetIndexToBeReplaced());
        }

        [HttpGet]
        public IActionResult UpdateMemory(Int32 currentIndex)
        {
            return Json(_applicationService.UpdateMemory(currentIndex));
        }
    }
}