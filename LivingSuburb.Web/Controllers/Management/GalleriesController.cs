using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Management.Controllers
{
    public class GalleriesController : Controller
    {
        private readonly ILogger<GalleriesController> logger;
        private GalleryService galleryService;
        public GalleriesController(
            ILogger<GalleriesController> logger,
            GalleryService galleryService
            )
        {
            this.logger = logger;
            this.galleryService = galleryService;
        }

        [AcceptVerbs("GET")]
        [Authorize(Roles = "Administrator")]
        [Route("Management/Galleries/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            this.logger.LogInformation("Edit Starts");
            TempData["Id"] = id;
            this.logger.LogInformation("Edit Stops");
            return View("~/Views/Management/Galleries/Edit.cshtml");
        }

        [AcceptVerbs("GET")]
        [Authorize(Roles = "Administrator")]
        [Route("Management/Galleries/Add")]
        public IActionResult Add()
        {
            this.logger.LogInformation("Add Starts");


            this.logger.LogInformation("Add Stops");
            return View("~/Views/Management/Galleries/Add.cshtml");
        }


        [AcceptVerbs("GET")]
        [Authorize(Roles = "Administrator")]
        [Route("Management/Galleries")]
        public IActionResult Index()
        {
            this.logger.LogInformation("Index Starts");


            this.logger.LogInformation("Index Stops");
            return View("~/Views/Management/Galleries/Index.cshtml");
        }
    }
}
