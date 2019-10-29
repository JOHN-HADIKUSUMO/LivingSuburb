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
    public class CountriesController : ControllerBase
    {
        private CountryService countryService;
        public CountriesController(CountryService countryService)
        {
            this.countryService = countryService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("API/COUNTRIES/LIST")]
        public IActionResult List()
        {
            List<SimpleItem> result = new List<SimpleItem>();
            result.Add(new SimpleItem(0, "Any Country"));
            result.AddRange(this.countryService.GetAll().Select(s => new SimpleItem{ Id = s.CountryId, Name = s.Name }).ToArray());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/COUNTRIES/SEARCH")]
        public IActionResult Search([FromBody] CountrySearch model)
        {
            CountryListResult result = this.countryService.Search(model.Keywords,model.OrderBy, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/COUNTRIES/SEARCH-BY-KEYWORD")]
        public IActionResult Search2([FromBody] SearchCountryKeywords model)
        {
            List<SearchCountryResult> result = this.countryService.SearchByKeyword(model.Keywords, model.Take);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/COUNTRIES/ADD")]
        public IActionResult Add([FromBody] Country model)
        {
            if (this.countryService.Create(model))
            {
                return Ok(true);
            }
            else
            {
                return Conflict("Duplication is not allowed.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/COUNTRIES/DELETE")]
        public IActionResult Delete([FromBody] Delete model)
        {
            if (this.countryService.Delete(model.Id))
            {
                
                return Ok(true);
            }
            else
            {
                return NotFound("Can't find a record with id = " + model.Id);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("API/COUNTRIES/READ/{id}")]
        public IActionResult Read(int id)
        {
            Country country = this.countryService.Read(id);
            if(country != null)
            {
                return Ok(country);
            }
            else
            {
                return NotFound("Can't find any record for id = " + id);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/COUNTRIES/UPDATE")]
        public IActionResult Update([FromBody] Country model)
        {
            if (this.countryService.Update(model))
            {
                return Ok(true);
            }
            else
            {
                return NotFound(@"Can't find """ + model.Name + @"""");
            }
        }
    }
}
