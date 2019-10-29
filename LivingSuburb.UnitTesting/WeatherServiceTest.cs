using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using NUnit.Framework;
using Serilog;
using Serilog.AspNetCore;
using LivingSuburb.Models;
using LivingSuburb.Database;
using LivingSuburb.Services;
using System;

namespace Tests
{
    public class WeatherServiceTest : Base
    {
        private HtmlDocument document;
        private WeatherService weatherService;
        private ILogger<WeatherService> logger;

        public WeatherServiceTest():base()
        {
            document = new HtmlDocument();
            logger = GetLogger(configuration, serviceProvider);
            weatherService = new WeatherService(context, configuration, logger);
        }

        private Microsoft.Extensions.Logging.ILogger<WeatherService> GetLogger(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            return (ILogger<WeatherService>)serviceProvider.GetService(typeof(ILogger<WeatherService>));
        }

        [SetUp]
        public void Setup()
        {
            
        }

        

        //private HtmlNode GetMainNode()
        //{
        //    string url = "https://www.eldersweather.com.au";
        //    HtmlWeb htmlWeb = new HtmlWeb();
        //    HtmlDocument htmlDocument = htmlWeb.Load(url);
        //    HtmlNode main = htmlDocument.GetElementbyId("fixed-layout");
        //    return main;
        //}


        //private HtmlNodeCollection GetCityNode(HtmlNode mainNode,string cityname)
        //{
        //    HtmlNodeCollection trNodes = mainNode.SelectNodes("tr");
        //    if(trNodes != null)
        //    {
        //        foreach(HtmlNode item in trNodes)
        //        {
        //            HtmlNodeCollection tdNodes = item.SelectNodes("td");
        //            if(tdNodes != null)
        //            {
        //                if (tdNodes[0].InnerText == cityname)
        //                    return tdNodes;
        //            }
        //        }
        //    }

        //    return null;
        //}

        //private OpenWeather GetWeather(string cityname)
        //{
        //    HtmlNode main = GetMainNode();
        //    HtmlNodeCollection collection = GetCityNode(main, cityname);
        //    OpenWeather model = new OpenWeather();
        //    model.City = collection[0].InnerText;
        //    model.IconTitle = collection[1].InnerText;
        //    HtmlNodeCollection imageCollection = collection[1].SelectNodes("img");
        //    if(imageCollection != null)
        //    {
        //        model.IconURL = "https://www.eldersweather.com.au" + imageCollection[0].Attributes["src"].Value;
        //    }
        //    var Temperature = Regex.Match(collection[3].InnerText, @"\d+");
        //    model.Temperature = System.Convert.ToDecimal(Temperature.Value);
        //    var Wind = Regex.Match(collection[9].InnerText, @"\d+");
        //    model.WindSpeed = System.Convert.ToDecimal(Wind.Value);


        //    return model;
        //}

        [TestCase]
        public async Task GetHTTPForex_Test()
        {
            var url = this.configuration.GetSection("OpenWeather:Url").Value;
            OpenWeather model =  await weatherService.GetHTTPWeather(City.Sydney.ToString());
            Assert.IsTrue(true);
        }
    }
}