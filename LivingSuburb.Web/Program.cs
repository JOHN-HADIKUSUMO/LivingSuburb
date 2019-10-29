using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.AspNetCore;

namespace LivingSuburb.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureKestrel((context, options) =>
            {

            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            })
            .UseSerilog((context, config)=> {
                config.ReadFrom.Configuration(context.Configuration);
            })
            .UseStartup<Startup>();
    }
}
