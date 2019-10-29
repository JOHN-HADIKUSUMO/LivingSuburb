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

namespace LivingSuburb.SportWinService
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
            ServiceName = "Sport News Service";
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
                RunSportNews();
            });
        }

        private async void RunSportNews()
        {
            logger.LogInformation("RunSportNews starts at " + Environment.TickCount);
            DataContext dataContext = GetDataContext();
            logger.LogInformation("dataContext == null ? " + (dataContext == null));
            NewsService newsService = new NewsService(dataContext, configuration, logger);
            if (newsService != null)
            {
                NewsAPIModel sport = await newsService.GetHTTPNews(CountriesEnum.Australia.ToString(), Category.Sport.ToString(), DateTime.Now.AddDays(-1), DateTime.Now);
                if (sport != null)
                {
                    for (int i = 0; i < sport.articles.Count() - 1; i++)
                    {
                        News news = new News();
                        news.NewsTitle = sport.articles[i].title;
                        news.NewsURL = sport.articles[i].url;
                        news.NewsSource = sport.articles[i].source.name;
                        news.NewsCategory = Category.Sport.ToString();
                        news.NewsCountry = CountriesEnum.Australia.ToString();
                        DateTime tempDate;
                        if (DateTime.TryParse(sport.articles[i].publishedAt, out tempDate))
                        {
                            news.DatePublished = tempDate;
                        }
                        bool result = await newsService.Create(news);
                    }
                }
            }
            logger.LogInformation("RunSportNews stops at " + Environment.TickCount);
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
