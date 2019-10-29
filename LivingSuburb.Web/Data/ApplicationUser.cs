using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LivingSuburb.Web.Data
{
    public class ApplicationUser:IdentityUser
    {
        public string Fullname { get; set; }
        public string URLAvatar { get; set; }
    }
   
}
