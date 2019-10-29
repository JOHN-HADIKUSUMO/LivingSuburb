using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class EventTypeService
    {
        private DataContext datacontext;
        public EventTypeService(DataContext context)
        {
            datacontext = context;
        }

        public EventType Read(string nameURL)
        {
            return datacontext.EventTypes.Where(w => w.NameURL.Trim().ToLower() == nameURL.Trim().ToLower()).FirstOrDefault();
        }

        public EventType Read(int id)
        {
            EventType result = null;
            if (datacontext.EventTypes.Where(w => w.EventTypeId == id).Any())
            {
                result = datacontext.EventTypes.Where(w => w.EventTypeId == id).FirstOrDefault();
            }
            return result;
        }

        public List<EventType> ReadByCategoryId(int id)
        {
            return datacontext.EventTypes.Join(datacontext.EventTypeCategories, a => a.EventTypeId, b => b.EventTypeId, (a, b) => new { a, b }).Where(w => w.b.EventCategoryId == id).OrderBy(o => o.a.Rank).Select(s => s.a).ToList();
        }

        public bool Create(EventType model)
        {
            bool status = false;
            if (!datacontext.EventTypes.Where(w=> w.EventTypeId == model.EventTypeId && w.Name == model.Name).Any())
            {
                datacontext.EventTypes.Add(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int id)
        {
            bool status = false;
            EventType model = datacontext.EventTypes.Where(w => w.EventTypeId == id).FirstOrDefault();
            if(model != null)
            {
                datacontext.EventTypes.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }
    }
}
