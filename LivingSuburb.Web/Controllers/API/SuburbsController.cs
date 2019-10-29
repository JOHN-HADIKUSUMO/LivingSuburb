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
    public class SuburbsController : ControllerBase
    {
        private SuburbService service;
        public SuburbsController(SuburbService service)
        {
            this.service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/SUBURBS/ADD-BULK")]
        public IActionResult AddBulk([FromBody] AddBulkSuburb model)
        {
            if (this.service.AddBulk(model))
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(false);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/SUBURBS/ADD")]
        public IActionResult Add([FromBody] AddSuburb model)
        {
            if(this.service.Add(model))
            {
                return Ok(true);
            }
            else
            {
                return Conflict("Duplication is not allowed.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("API/SUBURBS/READ/{id}")]
        public IActionResult Read(int id)
        {
            Suburb suburb = this.service.Read(id);
            if (suburb != null)
                return Ok(suburb);
            else
                return NotFound(false);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/SUBURBS/UPDATE")]
        public IActionResult Update([FromBody] Suburb model)
        {
            if (this.service.Update(model))
            {
                return Ok(true);
            }
            else
            {
                return NotFound("Can't find suburb with id = " + model.SuburbId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/SUBURBS/DELETE")]
        public IActionResult Delete([FromBody] DeleteSuburb model)
        {
            bool status = this.service.Delete(model.Id);
            return Ok(status);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("API/SUBURBS/SEARCH-LIST")]
        public IActionResult Search([FromBody] SearchSuburbList model)
        {
            SearchSuburbListResult result = this.service.Search(model.State, model.StartWith, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("API/SUBURBS/SEARCH")]
        public IActionResult Search([FromBody] SearchSuburbKeywords model)
        {
            List<SearchSuburbsResult> result = this.service.Search(model.Keywords, model.Take);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("API/SUBURBS/SEARCH2")]
        public IActionResult Search2([FromBody] SearchSuburbKeywords2 model)
        {
            List<SearchSuburbsResult> result = this.service.Search(model.Keywords, model.StateId, model.Take);
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("API/SUBURBS/DETAIL")]
        public IActionResult GetDetail(string state,string suburb)
        {
            SearchSuburbsDetail result = this.service.GetDetail(state, suburb);
            return Ok(result);
        }
    }
}
