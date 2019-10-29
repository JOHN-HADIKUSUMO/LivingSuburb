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
    public class TagsController : Controller
    {
        private TagService service;

        public TagsController(TagService service)
        {
            this.service = service;
        }

        [Route("Management/Tags")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {


            return View("~/Views/Management/Tags/Index.cshtml");
        }
    }
}
