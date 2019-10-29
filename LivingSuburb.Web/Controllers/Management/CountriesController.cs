using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Management.Controllers
{
    public class CountriesController : Controller
    {
        private CountryService service;

        public CountriesController(CountryService service)
        {
            this.service = service;
        }

        [Route("/Management/Countries")]
        public IActionResult Index()
        {
            return View("~/Views/Management/Countries/Index.cshtml");
        }

        [Route("/Management/Countries/Add")]
        public IActionResult Add()
        {
            return View("~/Views/Management/Countries/Add.cshtml");
        }

        [Route("/Management/Countries/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            TempData["Id"] = id;
            return View("~/Views/Management/Countries/Edit.cshtml");
        }
    }
}
