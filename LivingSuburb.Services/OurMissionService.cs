using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class OurMissionService
    {
        private DataContext datacontext;
        public OurMissionService(DataContext context)
        {
            datacontext = context;
        }
        
        public async Task<OurMission> Get()
        {
            return await datacontext.OurMissions.OrderBy(o=> o.OurMissionId).FirstOrDefaultAsync();
        }
    }
}
