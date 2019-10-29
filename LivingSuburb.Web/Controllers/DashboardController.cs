using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using LivingSuburb.Models;

namespace LivingSuburb.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "User")]
        [Route("/Dashboard/Jobs/Add")]
        public IActionResult JobsAdd()
        {
            _logger.LogInformation("Jobs Add Starts");


            _logger.LogInformation("Jobs Add Stops");
            return View();
        }

        [Authorize(Roles = "User")]
        [Route("/Dashboard/Jobs/List")]
        public IActionResult JobsList()
        {
            _logger.LogInformation("Jobs List Starts");


            _logger.LogInformation("Jobs List Stops");
            return View();
        }

        [Authorize(Roles = "User")]
        [Route("/Dashboard/Avatar")]
        public IActionResult Avatar()
        {
            _logger.LogInformation("Avatar Starts");


            _logger.LogInformation("Avatar Stops");
            return View();
        }

        [Authorize(Roles = "User")]
        [Route("/Dashboard/Profile")]
        public IActionResult Profile()
        {
            _logger.LogInformation("Profile Starts");


            _logger.LogInformation("Profile Stops");
            return View();
        }

        [Route("/Dashboard/Comments")]
        public IActionResult Comments()
        {
            _logger.LogInformation("Comments Starts");


            _logger.LogInformation("Comments Stops");
            return View();
        }
    }
}
