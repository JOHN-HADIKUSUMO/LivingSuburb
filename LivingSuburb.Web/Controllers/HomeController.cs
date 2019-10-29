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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("Privacy")]
        public IActionResult Privacy()
        {
            _logger.LogDebug("Privacy starts");
            return View();
        }
        [Route("Term")]
        public IActionResult Term()
        {
            _logger.LogDebug("Term starts");
            return View();
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index Starts");
            _logger.LogInformation(Directory.GetCurrentDirectory());
            bool isAuthenticate = User.Identity.IsAuthenticated;
            return View();
        }

        [Route("About-Us")]
        public IActionResult About()
        {
            _logger.LogWarning("About You");
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            _logger.LogDebug("Contact starts");

            ViewData["Message"] = "Your contact page.";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
