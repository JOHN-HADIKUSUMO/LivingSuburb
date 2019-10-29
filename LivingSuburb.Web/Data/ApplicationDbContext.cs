using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LivingSuburb.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration configuration;
        public ApplicationDbContext(IConfiguration config, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            configuration = config ?? throw new System.ArgumentNullException(nameof(config));
        }
    }
}
