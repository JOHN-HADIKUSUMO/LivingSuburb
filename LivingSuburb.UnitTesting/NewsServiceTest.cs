using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using LivingSuburb.Models;
using LivingSuburb.Database;
using LivingSuburb.Services;

namespace Tests
{
    public class NewsServiceTest : Base
    {
        private NewsService newsService;
        private ILogger<NewsService> logger;

        [SetUp]
        public void Setup()
        {
            newsService = new NewsService(context, configuration, logger);
        }

        [TestCase("Hello Buddy", "http://www.gizmo.com/Hello-Buddy","Kompas", "Australia", "Politic")]
        public void Create_Test(string title,string url,string source,string country,string category)
        {
            News model = new News();
            model.NewsTitle = title;
            model.NewsURL = url;
            model.NewsSource = source;
            model.NewsCountry = country;
            model.NewsCategory = category;
            Task<bool> result = newsService.Create(model);
            Assert.IsTrue(result.Result);
        }

        [TestCase("Australia","Technology",10)]
        public void Read_Test(string country, string category,int take)
        {
            Task<List<News>> test = newsService.Read(country,category,take);
            int count = test.Result.Count;
            Assert.IsTrue(count > 0);
        }

        [TestCase]
        public void Read_HTTP_Test()
        {
            Task<NewsAPIModel> test = newsService.GetHTTPNews("AU", "politics", DateTime.Now.AddDays(-1),DateTime.Now);
            int count = test.Result.articles.Count;
            Assert.IsTrue(count > 0);
        }
    }
}