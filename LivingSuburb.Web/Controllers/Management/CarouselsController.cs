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
    public class CarouselsController : Controller
    {
        private CarouselService service;

        public CarouselsController(CarouselService service)
        {
            this.service = service;
        }

        [Route("Management/Carousels")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {


            return View("~/Views/Management/Carousels/Index.cshtml");
        }

        [Route("Management/Carousels/Add")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Add()
        {


            return View("~/Views/Management/Carousels/Add.cshtml");
        }

        [Route("Management/Carousels/Edit/{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(int id)
        {

            TempData["Id"] = id;
            return View("~/Views/Management/Carousels/Edit.cshtml");
        }
    }
}
