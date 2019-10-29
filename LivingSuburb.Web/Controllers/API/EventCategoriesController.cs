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
    public class EventCategoriesController : ControllerBase
    {
        private EventCategoryService eventCategoryService;
        public EventCategoriesController(EventCategoryService eventCategoryService)
        {
            this.eventCategoryService = eventCategoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("API/EVENTCATEGORIES/LIST")]
        public IActionResult List()
        {
            List<SimpleItem> result = new List<SimpleItem>();
            List<EventCategory> temps = this.eventCategoryService.GetAll();
            if(temps != null)
            {
                result.Add(new SimpleItem(0, "Any Event Category"));
                result.AddRange(temps.Select(s => new SimpleItem(s.EventCategoryId, s.Name)).ToArray());
            }
            return Ok(result);
        }
    }
}
