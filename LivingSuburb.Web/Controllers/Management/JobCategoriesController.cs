using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Management.Controllers
{
    public class JobCategoriesController : Controller
    {
        private CategoryService service;

        public JobCategoriesController(CategoryService service)
        {
            this.service = service;
        }

        [AcceptVerbs("GET")]
        [Authorize(Roles = "Administrator")]
        [Route("/Management/JobCategories")]
        public IActionResult Index()
        {



            return View("~/Views/Management/JobCategories/Index.cshtml");
        }

        [Route("/Management/JobCategories/Add")]
        public IActionResult Add()
        {

            return View("~/Views/Management/JobCategories/Add.cshtml");
        }

        [Route("/Management/JobCategories/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            TempData["Id"] = id;
            return View("~/Views/Management/JobCategories/Edit.cshtml");
        }
    }
}
