using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using LivingSuburb.Web.Data;
using LivingSuburb.Models;
using LivingSuburb.Models.Account;
using LivingSuburb.Services;
using LivingSuburb.Services.Email;

namespace LivingSuburb.Web.Management.Controllers
{
    public class UsersController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;
        private readonly TemplateService _templateService;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _baseURL;
        public UsersController(
            IConfiguration _configuration,
            IHostingEnvironment _hostingEnvironment,
            TemplateService _templateService,
            UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager,
            ILogger<UsersController> logger
            )
        {
            this._configuration = _configuration;
            this._hostingEnvironment = _hostingEnvironment;
            this._templateService = _templateService;
            this._signInManager = _signInManager;
            this._userManager = _userManager;
            this._baseURL = this._configuration.GetSection("BaseURL").Value;
            this._logger = logger;
        }

        [Route("Management/Users")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {

            return View("~/Views/Management/Users/Index.cshtml");
        }
    }
}
