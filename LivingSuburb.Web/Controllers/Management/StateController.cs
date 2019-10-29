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
    public class StateController : Controller
    {
        private StateService service;
        public StateController(StateService service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            List<State> states = this.service.Search("New");
            return View();
        }

        public IActionResult Add()
        {

            return View();
        }
    }
}
