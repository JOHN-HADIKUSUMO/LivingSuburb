using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class LinkService
    {
        private DataContext datacontext;
        public LinkService(DataContext context)
        {
            datacontext = context;
        }
        
        public List<WeatherCoordinate> GetAll()
        {
            return datacontext.Links.Where(w => (bool)w.Enabled).ToList();
        }
    }
}
