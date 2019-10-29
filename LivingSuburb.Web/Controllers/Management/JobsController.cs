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
    public class JobsController : Controller
    {
        private TagService service;

        public JobsController(TagService service)
        {
            this.service = service;
        }

        [Route("Management/Jobs")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {


            return View("~/Views/Management/Jobs/Index.cshtml");
        }

        [Route("Management/Jobs/Add")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Add()
        {


            return View("~/Views/Management/Jobs/Add.cshtml");
        }

        [Route("Management/Jobs/Edit/{id}/{title}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(int id,string title)
        {

            TempData["Id"] = id;
            return View("~/Views/Management/Jobs/Edit.cshtml");
        }
    }
}
