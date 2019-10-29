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
    public class ErrorController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public ErrorController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{code}")]
        public IActionResult Index(string code)
        {
            switch(code)
            {
                case "400":
                    {
                        TempData["Label"] = "BAD REQUEST";
                        TempData["Code"] = code;
                    }
                    break;
                case "500":
                    {
                        TempData["Label"] = "INTERNAL SERVER ERROR";
                        TempData["Code"] = code;
                    }
                    break;
            }
            return View();
        }
    }
}
