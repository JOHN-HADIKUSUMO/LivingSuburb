using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Controllers.API
{
    public class CommonController : ControllerBase
    {
        public CommonController()
        {
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/COMMON/URL-FRIENDLY")]
        public IActionResult URLFriendly([FromBody] SearchKeywords model)
        {
            string url = Regex.Replace(model.Keywords, "[^0-9a-zA-Z]+", "-");
            return Ok(url);
        }
    }
}
