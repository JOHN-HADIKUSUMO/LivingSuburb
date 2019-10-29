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
    public class JobCategoriesController : ControllerBase
    {
        private CategoryService categoryService;
        public JobCategoriesController(CategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("API/JOB-CATEGORIES/LIST")]
        public IActionResult List()
        {
            List<SimpleItem> result = new List<SimpleItem>();
            result.Add(new SimpleItem(0, "Any Category"));
            result.AddRange(this.categoryService.GetAll().Select(s => new SimpleItem{ Id = s.CategoryId, Name = s.Name }).ToArray());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOB-CATEGORIES/SEARCH")]
        public IActionResult Search([FromBody] JobCategorySearch model)
        {
            JobCategoryListResult result = this.categoryService.Search(model.Keywords,model.OrderBy, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOB-CATEGORIES/ADD")]
        public IActionResult Add([FromBody] JobCategory model)
        {
            if (this.categoryService.Create(model))
            {
                return Ok();
            }
            else
            {
                return Conflict("Duplication is not allowed.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOB-CATEGORIES/DELETE")]
        public IActionResult Delete([FromBody] Delete model)
        {
            if (this.categoryService.Delete(model.Id))
            {
                
                return Ok();
            }
            else
            {
                return NotFound("Can't find a record with id = " + model.Id);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOB-CATEGORIES/READ/{id}")]
        public IActionResult Read(int id)
        {
            JobCategory jobCategory = this.categoryService.Read(id);
            if(jobCategory != null)
            {
                return Ok(jobCategory);
            }
            else
            {
                return NotFound("Can't find any record for id = " + id);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOB-CATEGORIES/UPDATE")]
        public IActionResult Update([FromBody] JobCategory model)
        {
            if (this.categoryService.Update(model))
            {
                return Ok();
            }
            else
            {
                return NotFound(@"Can't find """ + model.Name + @"""");
            }
        }
    }
}
