using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class TagGroupService
    {
        private DataContext datacontext;
        public TagGroupService(DataContext context)
        {
            datacontext = context;
        }

        public bool Create(TagGroup model)
        {
            bool status = false;
            if (!datacontext.TagGroups.Where(w=> w.TagGroupId == model.TagGroupId && w.Name.ToUpper() == model.Name.ToUpper()).Any())
            {
                datacontext.TagGroups.Add(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Update(TagGroup model)
        {
            bool status = false;
            if (datacontext.TagGroups.Where(w => w.TagGroupId == model.TagGroupId && w.Name.ToUpper() == model.Name.ToUpper()).Any())
            {
                datacontext.TagGroups.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(TagGroup model)
        {
            bool status = false;
            if (datacontext.TagGroups.Where(w => w.TagGroupId == model.TagGroupId && w.Name.ToUpper() == model.Name.ToUpper()).Any())
            {
                datacontext.TagGroups.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int id)
        {
            bool status = false;
            TagGroup tagGroup = datacontext.TagGroups.Where(w => w.TagGroupId == id).FirstOrDefault();
            if(tagGroup != null)
            {
                datacontext.TagGroups.Remove(tagGroup);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }
    }
}
