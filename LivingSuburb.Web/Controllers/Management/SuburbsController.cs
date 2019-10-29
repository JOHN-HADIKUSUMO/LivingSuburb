using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Management.Controllers
{
    public class SuburbsController : Controller
    {
        private SuburbService service;

        public SuburbsController(SuburbService service)
        {
            this.service = service;
        }

        [Route("Management/Suburbs")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {


            return View("~/Views/Management/Suburbs/Index.cshtml");
        }
        [Route("Management/Suburbs/Edit/{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(int id)
        {
            TempData["Id"] = id;
            return View("~/Views/Management/Suburbs/Edit.cshtml");
        }

        [Route("Management/Suburbs/Add")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Add()
        {


            return View("~/Views/Management/Suburbs/Add.cshtml");
        }

        [Route("Management/Suburbs/Add-Bulk")]
        [Authorize(Roles = "Administrator")]
        public IActionResult AddBulk()
        {


            return View("~/Views/Management/Suburbs/AddBulk.cshtml");
        }
    }
}
