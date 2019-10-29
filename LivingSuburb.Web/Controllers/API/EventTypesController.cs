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
    public class EventTypesController : ControllerBase
    {
        private EventTypeService eventTypeService;
        public EventTypesController(EventTypeService eventTypeService)
        {
            this.eventTypeService = eventTypeService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("API/EVENTTYPES/LIST/{id}")]
        public IActionResult List(int id)
        {
            List<SimpleItem> result = new List<SimpleItem>();
            List<EventType> temps = this.eventTypeService.ReadByCategoryId(id);
            if(temps != null)
            {
                result.Add(new SimpleItem(0, "Any Event Type"));
                result.AddRange(temps.Select(s => new SimpleItem(s.EventTypeId, s.Name)).ToArray());
            }
            return Ok(result);
        }
    }
}
