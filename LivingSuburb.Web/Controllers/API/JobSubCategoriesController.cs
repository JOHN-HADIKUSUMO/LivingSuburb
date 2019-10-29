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
    public class JobSubCategoriesController : ControllerBase
    {
        private SubCategoryService subCategoryService;
        public JobSubCategoriesController(SubCategoryService subCategoryService)
        {
            this.subCategoryService = subCategoryService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOB-SUB-CATEGORIES/UPDATE")]
        public IActionResult Update([FromBody] JobSubCategory model)
        {
            if (this.subCategoryService.Update(model))
            {
                return Ok();
            }
            else
            {
                return NotFound(@"Can't find """ + model.Name + @"""");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOB-SUB-CATEGORIES/ADD")]
        public IActionResult Add([FromBody] JobSubCategory model)
        {
            if (this.subCategoryService.Create(model))
            {
                return Ok();
            }
            else
            {
                return Conflict("Duplication is not allowed.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOB-SUB-CATEGORIES/READ/{id}")]
        public IActionResult Read(int id)
        {
            JobSubCategory jobSubCategory = this.subCategoryService.Read(id);
            if (jobSubCategory != null)
            {
                return Ok(jobSubCategory);
            }
            else
            {
                return NotFound("Can't find any record for id = " + id);
            }
        }

        [Authorize(Roles = "Administrator")]
        [Route("API/JOB-SUB-CATEGORIES/DELETE")]
        public IActionResult Delete([FromBody] Delete model)
        {
            if (this.subCategoryService.Delete(model.Id))
            {
                return Ok();
            }
            else
            {
                return NotFound("Can't find a record with id = " + model.Id);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("API/JOB-SUB-CATEGORIES/LIST/{id}")]
        public IActionResult List(int id)
        {
            List<SimpleItem> result = new List<SimpleItem>();
            result.Add(new SimpleItem(0, "Any Sub Category"));
            result.AddRange(this.subCategoryService.GetAll(id).Select(s => new SimpleItem { Id = s.JobSubCategoryId, Name = s.Name }).ToArray());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOB-SUB-CATEGORIES/SEARCH")]
        public IActionResult Search([FromBody] JobSubCategorySearch model)
        {
            JobSubCategoryListResult result = this.subCategoryService.Search(model.Keywords,model.Category, model.OrderBy, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }
    }
}
