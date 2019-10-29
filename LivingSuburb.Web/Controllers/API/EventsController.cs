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
    public class EventsController : ControllerBase
    {
        private CountryService countryService;
        private TagService tagService;
        private EventService eventService;
        private EventTypeService eventTypeService;
        private EventCategoryService eventCategoryService;
        private EventTagService eventTagService;

        public EventsController(EventTypeService eventTypeService, EventCategoryService eventCategoryService, CountryService countryService, TagService tagService, EventService eventService, EventTagService eventTagService)
        {
            this.eventTypeService = eventTypeService;
            this.eventCategoryService = eventCategoryService;
            this.countryService = countryService;
            this.tagService = tagService;
            this.eventService = eventService;
            this.eventTagService = eventTagService;
        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/EVENTS/DELETE")]
        public IActionResult Delete([FromBody] Delete model)
        {
            if (this.eventService.Delete(model.Id))
            {

                return Ok(true);
            }
            else
            {
                return NotFound("Can't find a record with id = " + model.Id);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/EVENTS/URL-FRIENDLY")]
        public IActionResult URLFriendly([FromBody] SearchKeywords model)
        {
            string url = Regex.Replace(model.Keywords, "[^0-9a-zA-Z]+", "-");
            return Ok(url);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/EVENTS/SEARCH")]
        public IActionResult Search([FromBody] EventSearch model)
        {
            EventListResult result = this.eventService.Search(model.Keywords, model.OrderBy, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("API/EVENTS/SEARCH2")]
        public IActionResult Search2([FromBody] EventSearch2 model)
        {
            EventListResult2 result = this.eventService.Search2(model.Keywords,model.CategoryId,model.EventTypeId, model.OrderBy, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/EVENTS/UPDATE")]
        public IActionResult Update([FromBody] EventUpdate model)
        {
            bool status = false;
            Event tempEvent = eventService.Read(model.EventId);
            if(tempEvent != null)
            {
                tempEvent.Title = model.Title;
                tempEvent.TitleURL = model.TitleURL;
                tempEvent.ShortDescription = model.ShortDescription;
                tempEvent.Description = model.FullDescription;
                tempEvent.CountryId = (int)model.Country.Id;
                tempEvent.Location = model.Location;
                tempEvent.EventCategoryId = (int)model.Category.Id;
                tempEvent.EventTypeId = (int)model.EventType.Id;
                tempEvent.From = model.FromDate;
                tempEvent.To = model.ToDate;
                tempEvent.DatePublished = model.DatePublished;
                tempEvent.SourceUrl = model.Url;

                eventService.Update(tempEvent);

                if (model.NewTags != null)
                {
                    if (model.NewTags.Count() > 0)
                    {
                        foreach (SimpleItem item in model.NewTags)
                        {
                            if (item.Id == null)
                            {
                                Tag tempTag = tagService.Read(item.Name, (int)TagGroupType.Event_Search);
                                if (tempTag == null)
                                {
                                    Tag tag = new Tag();
                                    tag.Name = item.Name;
                                    tag.TagGroupId = (int)TagGroupType.Event_Search;
                                    tagService.Create(tag);
                                    item.Id = tag.TagId;
                                }
                                else
                                {
                                    item.Id = tempTag.TagId;
                                }
                            }

                            EventTag eventTag = new EventTag();
                            eventTag.EventId = tempEvent.EventId;
                            eventTag.TagId = (int)item.Id;
                            eventTagService.Create(eventTag);
                        }
                    }
                }

                if(model.DeletedTags != null)
                {
                    if (model.DeletedTags.Count() > 0)
                    {
                        foreach (SimpleItem item in model.DeletedTags)
                        {
                            EventTag eventTag = new EventTag();
                            eventTag.EventId = tempEvent.EventId;
                            eventTag.TagId = (int)item.Id;
                            eventTagService.Delete(eventTag);
                        }
                    }
                }

                status = true;
            }

            if (status)
                return Ok(status);
            else
                return NotFound(status);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/EVENTS/ADD")]
        public IActionResult Add([FromBody] EventAdd model)
        {
            Event temp = new Event();
            temp.Title = model.Title;
            temp.TitleURL = model.TitleURL;
            temp.ShortDescription = model.ShortDescription;
            temp.Description = model.FullDescription;
            temp.CountryId = (int)model.Country.Id;
            temp.Location = model.Location;
            temp.EventCategoryId = (int)model.Category.Id;
            temp.EventTypeId = (int)model.EventType.Id;
            temp.From = model.FromDate;
            temp.To = model.ToDate;
            temp.DatePublished = model.DatePublished;
            temp.SourceUrl = model.Url;

            bool status = eventService.Create(temp);

            if (model.NewTags != null)
            {
                if (model.NewTags.Count() > 0)
                {
                    foreach (SimpleItem item in model.NewTags)
                    {
                        if (item.Id == null)
                        {
                            Tag tempTag = tagService.Read(item.Name, (int)TagGroupType.Event_Search);
                            if (tempTag == null)
                            {
                                Tag tag = new Tag();
                                tag.Name = item.Name;
                                tag.TagGroupId = (int)TagGroupType.Event_Search;
                                tagService.Create(tag);
                                item.Id = tag.TagId;
                            }
                            else
                            {
                                item.Id = tempTag.TagId;
                            }
                        }

                        EventTag eventTag = new EventTag();
                        eventTag.EventId = temp.EventId;
                        eventTag.TagId = (int)item.Id;
                        eventTagService.Create(eventTag);
                    }
                }

                if (model.DeletedTags.Count() > 0)
                {
                    foreach (SimpleItem item in model.DeletedTags)
                    {
                        EventTag eventTag = new EventTag();
                        eventTag.EventId = temp.EventId;
                        eventTag.TagId = (int)item.Id;
                        eventTagService.Delete(eventTag);
                    }
                }
            }

            return new StatusCodeResult(status ? 200 : 409);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("API/EVENTS/EDIT/{id}")]
        public IActionResult Edit(int id)
        {
            EventEdit result = new EventEdit();

            Event tempEvent = eventService.Read(id);
            if (tempEvent != null)
            {
                result.EventId = tempEvent.EventId;
                result.Title = tempEvent.Title;
                result.TitleURL = tempEvent.TitleURL;
                result.ShortDescription = tempEvent.ShortDescription;
                result.FullDescription = tempEvent.Description;

                Country tempCountry = countryService.Read(tempEvent.CountryId);
                if (tempCountry != null)
                {
                    result.Country = new SimpleItem(tempCountry.CountryId, tempCountry.Name);
                }

                result.Location = tempEvent.Location;

                EventCategory tempCategory = eventCategoryService.Read(tempEvent.EventCategoryId);
                if (tempCategory != null)
                {
                    result.Category = new SimpleItem(tempCategory.EventCategoryId, tempCategory.Name);
                }

                EventType tempType = eventTypeService.Read(tempEvent.EventTypeId);
                if (tempType != null)
                {
                    result.EventType = new SimpleItem(tempType.EventTypeId, tempType.Name);
                }

                result.FromDate = tempEvent.From;
                result.FromDateStr = string.Format("{0:yyyy-MM-dd HH:mm}", tempEvent.From);
                result.ToDate = tempEvent.To;
                result.ToDateStr = string.Format("{0:yyyy-MM-dd HH:mm}", tempEvent.To);
                result.DatePublished = tempEvent.DatePublished;
                result.DatePublishedStr = string.Format("{0:yyyy-MM-dd HH:mm}", tempEvent.DatePublished);
                result.Url = tempEvent.SourceUrl;
                result.ShortDescription = tempEvent.ShortDescription;
                result.FullDescription = tempEvent.Description;
                result.DatePublished = tempEvent.DatePublished;

                List<EventTag> eventTags = eventTagService.ReadAllByEventId(id);
                if (eventTags != null)
                {
                    if (eventTags.Count() > 0)
                    {
                        for (int i = 0; i < eventTags.Count; i++)
                        {
                            Tag tag = tagService.Read(eventTags[i].TagId);
                            if (tag != null)
                            {
                                result.EditTags.Add(new SimpleItem(tag.TagId, tag.Name));
                            }
                        }
                    }
                }
                return Ok(result);
            }
            return NotFound(false);
        }
    }
}
