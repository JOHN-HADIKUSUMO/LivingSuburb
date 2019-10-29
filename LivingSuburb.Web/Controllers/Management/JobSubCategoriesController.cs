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
    public class JobSubCategoriesController : Controller
    {
        private CategoryService service;

        public JobSubCategoriesController(CategoryService service)
        {
            this.service = service;
        }

        [Route("/Management/JobSubCategories")]
        public IActionResult Index()
        {
            return View("~/Views/Management/JobSubCategories/Index.cshtml");
        }

        [Route("/Management/JobSubCategories/Add")]
        public IActionResult Add()
        {

            return View("~/Views/Management/JobSubCategories/Add.cshtml");
        }

        [Route("/Management/JobSubCategories/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            TempData["Id"] = id;
            return View("~/Views/Management/JobSubCategories/Edit.cshtml");
        }
    }
}
