using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Controllers
{
    public class SuburbsController : Controller
    {
        private SuburbService service;

        public SuburbsController(SuburbService service)
        {
            this.service = service;
        }

        [Route("Suburbs")]
        public IActionResult Index()
        {


            return View();
        }

        [Route("Suburbs/{statename}")]
        public IActionResult List(string statename)
        {
            SuburbsList model = new SuburbsList();
            model.StateName = statename;
             
            return View(model);
        }

        [Route("Suburbs/{statename}/{suburbname}")]
        public IActionResult Detail(string statename,string suburbname)
        {
            SearchSuburbsDetail model = this.service.GetDetail(statename, suburbname);
            return View(model);
        }
    }
}
