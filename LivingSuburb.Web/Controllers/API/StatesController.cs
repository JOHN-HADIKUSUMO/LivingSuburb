using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Controllers.API
{
    public class StatesController : ControllerBase
    {
        private StateService stateService;
        public StatesController(StateService stateService)
        {
            this.stateService = stateService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("API/STATES/LIST")]
        public IActionResult List()
        {
            List<SimpleItem> result = new List<SimpleItem>();
            result.Add(new SimpleItem(0, "Any State"));
            result.AddRange(this.stateService.GetAll().Select(s => new SimpleItem { Id = s.StateId, Name = s.LongName }).ToArray());
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("API/STATES/LIST2")]
        public IActionResult List2()
        {
            var result = new List<object>() { new { Id = 0, Name = "Any State", Code = string.Empty } };
            result.AddRange(this.stateService.GetAll().Select(s => new { Id = s.StateId, Name = s.LongName, Code = s.ShortName }).ToArray());
            return Ok(result);
        }
    }
}
