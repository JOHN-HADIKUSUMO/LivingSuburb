using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using LivingSuburb.Models;
using LivingSuburb.Database;
using LivingSuburb.Services;

namespace Tests
{
    public class Base
    {
        protected DataContext context;
        protected IConfiguration configuration;
        protected IServiceProvider serviceProvider;
        public Base()
        {
            configuration = GetConfiguration();
            serviceProvider = GetServiceProvider(configuration);
            context = GetDataContext();
            InitLogger(configuration);
        }

        private void InitLogger(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        }

        private IServiceProvider GetServiceProvider(IConfiguration configuration)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(f => f.AddSerilog());
            return serviceCollection.BuildServiceProvider();
        }

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
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
    }
}