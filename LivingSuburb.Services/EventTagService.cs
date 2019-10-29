using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class EventTagService
    {
        private DataContext datacontext;
        public EventTagService(DataContext context)
        {
            datacontext = context;
        }


        public EventTag Read(int id)
        {
            EventTag result = null;
            if (datacontext.EventTags.Where(w => w.EventId == id).Any())
            {
                result = datacontext.EventTags.Where(w => w.EventId == id).FirstOrDefault();
            }
            return result;
        }

        public bool Create(EventTag model)
        {
            bool status = false;
            if (!datacontext.EventTags.Where(w=> w.EventId == model.EventId && w.TagId == model.TagId).Any())
            {
                datacontext.EventTags.Add(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(EventTag model)
        {
            bool status = false;
            if(datacontext.EventTags.Where(w=> w.EventId == model.EventId && w.TagId == model.TagId).Any())
            {
                datacontext.EventTags.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int id)
        {
            bool status = false;
            EventTag model = datacontext.EventTags.Where(w => w.EventId == id).FirstOrDefault();
            if(model != null)
            {
                datacontext.EventTags.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public List<EventTag> ReadAllByEventId(int id)
        {
            return datacontext.EventTags.Where(w => w.EventId == id).ToList();
        }
    }
}
