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
        public void FullyAssociativeCache(SimulationParameters simulationParameters)
        {
            FullyAssociativeCacheApplicationService.SetupSimulation(simulationParameters);
        }

        public IActionResult GetCurrentAddressBreakdown()
        {
            return Json(_applicationService.GetCurrentAddressBreakdown());
        }
    }
}