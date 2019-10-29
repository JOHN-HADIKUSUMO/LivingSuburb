using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class TemplateService
    {
        private DataContext datacontext;
        public TemplateService(DataContext context)
        {
            datacontext = context;
        }

        public Template Read(int id)
        {
            Template result = null;
            if (datacontext.Templates.Where(w => w.Id == id).Any())
            {
                result = datacontext.Templates.Where(w => w.Id == id).FirstOrDefault();
            }
            return result;
        }

        public Template Read(string name)
        {
            Template result = null;
            if (datacontext.Templates.Where(w => w.Name == name).Any())
            {
                result = datacontext.Templates.Where(w => w.Name == name).FirstOrDefault();
            }
            return result;
        }

        public bool Create(Template model)
        {
            bool status = false;
            if (!datacontext.Templates.Where(w => w.Name == model.Name).Any())
            {
                datacontext.Templates.Add(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }


        public bool Update(Template model)
        {
            bool status = false;
            if (datacontext.Templates.Where(w => w.Name == model.Name).Any())
            {
                datacontext.Templates.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(Template model)
        {
            bool status = false;
            if (datacontext.Templates.Where(w => w.Name == model.Name).Any())
            {
                datacontext.Templates.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(string name)
        {
            bool status = false;
            Template template = datacontext.Templates.Where(w => w.Name == name).FirstOrDefault();
            if (template != null)
            {
                datacontext.Templates.Remove(template);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }
    }

}
