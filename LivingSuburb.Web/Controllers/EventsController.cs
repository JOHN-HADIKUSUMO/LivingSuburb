using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Controllers
{
    [AllowAnonymous]
    public class EventsController : Controller
    {
        private EventService eventService;
        private EventCategoryService eventCategoryService;
        private EventTypeService eventTypeService;
        public EventsController(EventCategoryService eventCategoryService, EventTypeService eventTypeService, EventService eventService)
        {
            this.eventCategoryService = eventCategoryService;
            this.eventTypeService = eventTypeService;
            this.eventService = eventService;
        }

        [Route("Events")]
        public IActionResult Index()
        {
            TempData["categoryId"] = null;
            TempData["categoryName"] = null;
            TempData["categoryNameURL"] = null;
            TempData["eventTypeId"] = null;
            TempData["eventTypeName"] = null;
            TempData["eventTypeNameURL"] = null;
            return View();
        }

        [Route("Events/Detail/{id}/{titleURL}")]
        public IActionResult Detail(int id,string titleURL)
        {
            EventDetail detail = this.eventService.ReadDetail(id);
            ViewData["Title"] = detail.Title;
            ViewData["Description"] = detail.Title + "," + detail.Category + (string.IsNullOrEmpty(detail.Location)?"": ("," + detail.Location)) + (detail.Tags.Count() > 0? "," + string.Join(',',detail.Tags):"");
            return View(detail);
        }

        [Route("Events/{categoryURL}")]
        public IActionResult Index(string categoryURL)
        {
            EventCategory eventCategory = this.eventCategoryService.Read(categoryURL);
            if (eventCategory != null)
            {
                TempData["categoryId"] = eventCategory.EventCategoryId;
                TempData["categoryName"] = eventCategory.Name;
                TempData["categoryNameURL"] = eventCategory.NameURL;
            }

            TempData["eventTypeId"] = null;
            TempData["eventTypeName"] = null;
            TempData["eventTypeNameURL"] = null;
            return View();
        }

        [Route("Events/{categoryURL}/{eventTypeURL}")]
        public IActionResult Index(string categoryURL, string eventTypeURL)
        {
            EventCategory eventCategory = this.eventCategoryService.Read(categoryURL);
            if (eventCategory != null)
            {
                TempData["categoryId"] = eventCategory.EventCategoryId;
                TempData["categoryName"] = eventCategory.Name;
                TempData["categoryNameURL"] = eventCategory.NameURL;
            }

            EventType eventType = this.eventTypeService.Read(eventTypeURL);
            if (eventType != null)
            {
                TempData["eventTypeId"] = eventType.EventTypeId;
                TempData["eventTypeName"] = eventType.Name;
                TempData["eventTypeNameURL"] = eventType.NameURL;
            }
            return View();
        }
    }
}
