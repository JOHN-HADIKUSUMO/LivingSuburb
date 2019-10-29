using System;
using System.Web;
using System.IO;
using System.Timers;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.ServiceProcess;
using Serilog;
using Serilog.AspNetCore;
using LivingSuburb.Models;
using LivingSuburb.Database;
using LivingSuburb.Services;

namespace LivingSuburb.WeatherWinService
{
    public class RunServices:ServiceBase
    {
        private IConfiguration configuration;
        private IServiceProvider serviceProvider;
        private Microsoft.Extensions.Logging.ILogger<WeatherService> logger;
        private Timer timer = null;
        private int delay = 0;

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        }

        private IServiceProvider GetServiceProvider(IConfiguration configuration)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(f => f.AddSerilog());
            return serviceCollection.BuildServiceProvider();
        }

        private Microsoft.Extensions.Logging.ILogger<WeatherService> GetLogger(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(f => f.AddSerilog());
            return serviceProvider.GetService<ILogger<WeatherService>>();
        }

        public RunServices()
        {
            ServiceName = "Weather Service";
            configuration = GetConfiguration();
            serviceProvider = GetServiceProvider(configuration);
            logger = GetLogger(configuration, serviceProvider);
        }

        protected override void OnStart(string[] args)
        {
            delay = GetDelay();
            timer = new Timer(delay);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
        }

        private string GetCurrentDirectory()
        {
            string fullPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string directoryPath = Path.GetDirectoryName(fullPath);
            return directoryPath;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() => {
                RunOpenWeather();
            });
        }

        private async void RunOpenWeather()
        {
            logger.LogInformation("Run Weather Service starts at " + Environment.TickCount);
            DataContext dataContext = GetDataContext();
            logger.LogInformation("dataContext == null ? " + (dataContext == null));
            WeatherService weatherService = new WeatherService(dataContext, configuration, logger);
            OpenWeather sydneyWeather = await weatherService.GetHTTPWeather(City.Sydney);
            if (sydneyWeather != null)
            {
                logger.LogInformation("sydneyWeather != null");
                await weatherService.Update(sydneyWeather);
            }
            else
            {
                logger.LogInformation("sydneyWeather == null");
            }

            OpenWeather perthWeather = await weatherService.GetHTTPWeather(City.Perth);
            if (perthWeather != null)
            {
                logger.LogInformation("perthWeather != null");
                await weatherService.Update(perthWeather);
            }
            else
            {
                logger.LogInformation("perthWeather == null");
            }

            OpenWeather melbourneWeather = await weatherService.GetHTTPWeather(City.Melbourne);
            if (melbourneWeather != null)
            {
                logger.LogInformation("melbourneWeather != null");
                await weatherService.Update(melbourneWeather);
            }
            else
            {
                logger.LogInformation("melbourneWeather == null");
            }

            OpenWeather hobartWeather = await weatherService.GetHTTPWeather(City.Hobart);
            if (hobartWeather != null)
            {
                logger.LogInformation("hobartWeather == null");
                await weatherService.Update(hobartWeather);
            }
            else
            {
                logger.LogInformation("hobartWeather != null");
            }

            OpenWeather darwinWeather = await weatherService.GetHTTPWeather(City.Darwin);
            if (darwinWeather != null)
            {
                logger.LogInformation("darwinWeather != null");
                await weatherService.Update(darwinWeather);
            }
            else
            {
                logger.LogInformation("darwinWeather == null");
            }

            OpenWeather canberraWeather = await weatherService.GetHTTPWeather(City.Canberra);
            if (canberraWeather != null)
            {
                logger.LogInformation("canberraWeather != null");
                await weatherService.Update(canberraWeather);
            }
            else
            {
                logger.LogInformation("canberraWeather == null");
            }


            OpenWeather brisbaneWeather = await weatherService.GetHTTPWeather(City.Brisbane);
            if (brisbaneWeather != null)
            {
                logger.LogInformation("brisbaneWeather != null");
                await weatherService.Update(brisbaneWeather);
            }
            else
            {
                logger.LogInformation("brisbaneWeather == null");
            }

            OpenWeather adelaideWeather = await weatherService.GetHTTPWeather(City.Adelaide);
            if (adelaideWeather != null)
            {
                logger.LogInformation("adelaideWeather != null");
                await weatherService.Update(adelaideWeather);
            }
            else
            {
                logger.LogInformation("adelaideWeather == null");
            }
            logger.LogInformation("Run Weather Service stops at " + Environment.TickCount);
        }

        private DataContext GetDataContext()
        {
            DataContext dataContext = null;
            if (configuration != null)
            {
                DbContextOptions<DataContext> dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")).Options;

                if (dbContextOptions != null)
                {
                    dataContext = new DataContext(configuration, dbContextOptions);
                }
            }
            return dataContext;
        }

        private int GetDelay()
        {
            logger.LogInformation("GetDelay starts");
            int delay = 0;
            IEnumerable<IConfigurationSection> sections = configuration.GetChildren();
            IConfigurationSection timerSection = sections.Where(w => w.Path == "Timer").Select(s => s).FirstOrDefault();
            if (timerSection != null)
            {
                IConfigurationSection delaySection = timerSection.GetSection("Delay");
                if (delaySection != null)
                {
                    delay = int.Parse(delaySection.Value);
                }
            }
            logger.LogInformation("delay => " + delay);
            logger.LogInformation("GetDelay stops");
            return delay;
        }

    }
}
