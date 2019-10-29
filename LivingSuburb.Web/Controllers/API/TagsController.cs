using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Controllers.API
{
    public class TagsController : ControllerBase
    {
        private TagService tagService;
        public TagsController(TagService tagService)
        {
            this.tagService = tagService;
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("API/TAGS/SEARCH-BY-KEYWORD-ONLY")]
        public IActionResult Search([FromBody] SearchTagKeywords model)
        {
            List<string> result = this.tagService.SearchByKO(model.GroupId,model.Keywords,model.Take);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("API/TAGS/SEARCH-BY-KEYWORD-ONLY2")]
        public IActionResult Search2([FromBody] SearchTagKeywords model)
        {
            List<SearchTagResult> result = this.tagService.SearchByKO2(model.GroupId, model.Keywords, model.Take);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/TAGS/SEARCH")]
        public IActionResult Search([FromBody] SearchTagList model)
        {
            SearchTagListResult result = this.tagService.Search(model.TagGroupId, model.StartWith, model.Keywords, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/TAGS/DELETE")]
        public IActionResult Delete([FromBody] DeleteTag model)
        {
            bool status = this.tagService.Delete(model.Id);
            return Ok(status);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/TAGS/CREATE")]
        public IActionResult Create([FromBody] Tag model)
        {
            ActionResult result;
            bool status = this.tagService.Create(model);
            if (status)
                result = Ok(@"""" + model.Name + @""" has been successfully saved.");
            else
                result = new ConflictObjectResult(@"""" + model.Name + @""" has existed in database.");
            return result;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/TAGS/UPDATE")]
        public IActionResult Update([FromBody] Tag model)
        {
            ActionResult result;
            bool status = this.tagService.Update(model);
            if (status)
                result = Ok();
            else
                result = new  StatusCodeResult(500);
            return result;
        }
    }
}
