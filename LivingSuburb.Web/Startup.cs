using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Amazon.S3;
using Microsoft.Win32;
using LivingSuburb.Database;
using LivingSuburb.Services;
using LivingSuburb.Services.Email;
using LivingSuburb.Web.Data;
using LivingSuburb.Web.TagHelpers;

namespace LivingSuburb.Web
{
    public class Startup
    {
        private string connectionString;
        private string securityKey;
        private string validIssuer;
        private string validAudience;
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            connectionString = this.configuration.GetConnectionString("DefaultConnection");
            securityKey = this.configuration.GetSection("JWTAuthentication:SecretKey").ToString();
            validIssuer = this.configuration.GetSection("JWTAuthentication:ValidIssuer").ToString();
            validAudience = this.configuration.GetSection("JWTAuthentication:ValidAudience").ToString();
            services.AddLogging();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                }
            );
            services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                }
            );
            services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.SignIn.RequireConfirmedEmail = true; })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;
            });

            byte[] arrayOfBytes = Encoding.ASCII.GetBytes(securityKey);
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(arrayOfBytes);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = validIssuer,
                    ValidAudience = validAudience,
                    IssuerSigningKey = symmetricSecurityKey
                };
             });

            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = this.configuration["OAuth.Facebook:Key"];// "882883545234935";
                options.AppSecret = this.configuration["OAuth.Facebook:Secret"]; //"9dc93f6deecf33c2765c404fc87f6a25";
                options.CallbackPath = new PathString("/signin-facebook");
            });

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = this.configuration["OAuth.Google:Key"]; //"1005909858554-2vh3t7fdujgsvd5f0hsuivpksjpb4425.apps.googleusercontent.com";
                options.ClientSecret = this.configuration["OAuth.Google:Secret"]; //"4vwO66Fn3pMDaInLEEFE0aia";
            });

            services.AddTransient(typeof(TempService));
            services.AddTransient(typeof(AfterRegistration));
            services.AddTransient(typeof(TemplateService));
            services.AddTransient(typeof(EventCategoryService));
            services.AddTransient(typeof(EventTypeService));
            services.AddTransient(typeof(EventService));
            services.AddTransient(typeof(EventTagService));
            services.AddTransient(typeof(CountryService));
            services.AddTransient(typeof(StateService));
            services.AddTransient(typeof(SuburbService));
            services.AddTransient(typeof(LinkService));
            services.AddTransient(typeof(CarouselService));
            services.AddTransient(typeof(WeatherService));
            services.AddTransient(typeof(OurMissionService));
            services.AddTransient(typeof(ForexService));
            services.AddTransient(typeof(PreciousMetalService));
            services.AddTransient(typeof(NewsService));
            services.AddTransient(typeof(CategoryService));
            services.AddTransient(typeof(SubCategoryService));
            services.AddTransient(typeof(TagService));
            services.AddTransient(typeof(JobService));
            services.AddTransient(typeof(JobTagService));
            services.AddTransient(typeof(GalleryService));
            services.AddTransient<IMemoryCache, MemoryCache>();
            services.AddTransient<ITagHelper, ContactUsTagHelper>();
            services.AddTransient<ITagHelper, LinksTagHelper>();
            services.AddTransient<ITagHelper, MissionsTagHelper>();
            services.AddTransient<ITagHelper, NewsTagHelper>();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseSession();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
