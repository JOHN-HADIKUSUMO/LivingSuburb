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
    public class EventsController : Controller
    {
        private TagService service;

        public EventsController(TagService service)
        {
            this.service = service;
        }

        [Route("Management/Events")]
        [Authorize(Roles = "Manager,Administrator")]
        public IActionResult Index()
        {
            return View("~/Views/Management/Events/Index.cshtml");
        }

        [Route("Management/Events/Add")]
        [Authorize(Roles = "Manager,Administrator")]
        public IActionResult Add()
        {


            return View("~/Views/Management/Events/Add.cshtml");
        }

        [Route("Management/Events/Edit/{id}")]
        [Authorize(Roles = "Manager,Administrator")]
        public IActionResult Edit(int id)
        {

            TempData["Id"] = id;
            return View("~/Views/Management/Events/Edit.cshtml");
        }
    }
}
