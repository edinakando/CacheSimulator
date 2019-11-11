using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CacheSimulator.Controllers
{
    public class FullyAssociativeCacheSimulationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}