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

namespace LivingSuburb.TechnologyWinService
{
    public class RunServices:ServiceBase
    {
        private IConfiguration configuration;
        private IServiceProvider serviceProvider;
        private Microsoft.Extensions.Logging.ILogger<NewsService> logger;
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

        private Microsoft.Extensions.Logging.ILogger<NewsService> GetLogger(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(f => f.AddSerilog());
            return serviceProvider.GetService<ILogger<NewsService>>();
        }

        public RunServices()
        {
            ServiceName = "Technology News Service";
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
                RunTechnologyNews();
            });
        }

        private async void RunTechnologyNews()
        {
            logger.LogInformation("RunTechnologyNews starts at " + Environment.TickCount);
            DataContext dataContext = GetDataContext();
            logger.LogInformation("dataContext == null ? " + (dataContext == null));
            NewsService newsService = new NewsService(dataContext, configuration, logger);
            if (newsService != null)
            {
                logger.LogInformation("newsService != null");
                NewsAPIModel technology = await newsService.GetHTTPNews(CountriesEnum.Australia.ToString(), Category.Technology.ToString(), DateTime.Now.AddDays(-1), DateTime.Now);
                if (technology != null)
                {
                    logger.LogInformation("technology != null");
                    for (int i = 0; i < technology.articles.Count() - 1; i++)
                    {
                        News news = new News();
                        news.NewsTitle = technology.articles[i].title;
                        news.NewsURL = technology.articles[i].url;
                        news.NewsSource = technology.articles[i].source.name;
                        news.NewsCategory = Category.Technology.ToString();
                        news.NewsCountry = CountriesEnum.Australia.ToString();
                        DateTime tempDate;
                        if (DateTime.TryParse(technology.articles[i].publishedAt, out tempDate))
                        {
                            news.DatePublished = tempDate;
                        }
                        bool result = await newsService.Create(news);
                        logger.LogInformation("result No. " + i.ToString() + " = " + result);
                    }
                }
            }
            logger.LogInformation("RunTechnologyNews stops at " + Environment.TickCount);
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
