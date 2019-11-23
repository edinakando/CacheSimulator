﻿using CacheSimulator.ApplicationService;
using CacheSimulator.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CacheSimulator.Controllers
{
    public class SetAssociativeCacheSimulationController : Controller
    {
        private readonly SetAssociativeCacheApplicationService _applicationService;

        public SetAssociativeCacheSimulationController(SetAssociativeCacheApplicationService applicationService)
        {
            _applicationService = applicationService;
        }
        [HttpGet]
        public IActionResult Index(Int32 setCount)
        {
            if(setCount == 2)
            {
                return View("2WaySetAssociativeCache");
            }
            else
            {
                return View("4WaySetAssociativeCache");
            }
        }

        [HttpPost]
        public void FullyAssociativeCache(SimulationParameters simulationParameters)
        {
            FullyAssociativeCacheApplicationService.SetupSimulation(simulationParameters);
           // return Json(_applicationService.GetIndexCount());
        }
    }
}