using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class WeatherService
    {
        private DataContext datacontext;
        private HttpClient client;
        private IConfiguration configuration;
        private ILogger<WeatherService> logger;
        private string url;
        public WeatherService(DataContext datacontext, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            this.logger = logger;
            this.logger.LogWarning("WeatherService Starts");
            this.datacontext = datacontext;
            this.configuration = configuration;
            this.client = new HttpClient();
            url = this.configuration["OpenWeather:Url"];
            this.logger.LogWarning("WeatherService Stops");
        }


        private Task<bool> isEmptyTable(string city)
        {
            this.logger.LogWarning("isEmptyTable Starts");
            bool status = this.datacontext.OpenWeathers.Where(w=> w.City.ToUpper().Trim() == city.ToUpper().Trim()).Any();
            this.logger.LogWarning("isEmptyTable Stops");
            return Task.FromResult<bool>(!status);
        }

        private void Create(OpenWeather model)
        {
            this.logger.LogWarning("Create Starts");
            this.datacontext.OpenWeathers.Add(model);
            this.datacontext.SaveChanges();
            this.logger.LogWarning("Create Stops");
        }

        public async Task<OpenWeather> Read(string city)
        {
            this.logger.LogWarning("Read Starts");
            OpenWeather openWeather = await this.datacontext.OpenWeathers.Where(w=> w.City.Trim().ToUpper() == city.Trim().ToUpper()).FirstOrDefaultAsync();
            this.logger.LogWarning("Read Stops");
            return openWeather;
        }

        public async Task<bool> Update(OpenWeather model)
        {
            this.logger.LogWarning("Update Starts");
            this.logger.LogWarning("model.City => " + model.City);
            bool result = false;
            bool isEmpty = await isEmptyTable(model.City);
            if (isEmpty)
            {
                Create(model);
            }
            this.logger.LogWarning("isEmpty => " + isEmpty.ToString());
            OpenWeather temp = this.datacontext.OpenWeathers.Where(w => w.City.ToUpper().Trim() == model.City.ToUpper().Trim()).FirstOrDefault();
            this.logger.LogWarning("temp == null => " + (temp == null).ToString());
            if (temp != null)
            {
                temp.City = model.City;
                temp.IconTitle = model.IconTitle;
                temp.IconURL = model.IconURL;
                temp.Temperature = model.Temperature;
                temp.WindSpeed = model.WindSpeed;
                temp.LastUpdate = DateTime.Now;

                this.datacontext.OpenWeathers.Update(temp);
                this.datacontext.SaveChanges();
                result = true;
            }
            this.logger.LogWarning("result => " + result.ToString());
            this.logger.LogWarning("Update Stops");
            return result;
        }

        private HtmlNode GetMainNode()
        {
            this.logger.LogWarning("GetMainNode starts");
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(url);
            HtmlNode main = htmlDocument.GetElementbyId("fixed-layout");
            this.logger.LogWarning("GetMainNode stops");
            return main;
        }


        private HtmlNodeCollection GetCityNode(HtmlNode mainNode, string city)
        {
            this.logger.LogWarning("GetCityNode starts");
            this.logger.LogWarning("city => " + city);
            if (mainNode != null)
            {
                this.logger.LogWarning("mainNode != null");
                HtmlNodeCollection trNodes = mainNode.SelectNodes("tr");
                if (trNodes != null)
                {
                    this.logger.LogWarning("trNodes != null");
                    foreach (HtmlNode item in trNodes)
                    {
                        HtmlNodeCollection tdNodes = item.SelectNodes("td");
                        if (tdNodes != null)
                        {
                            if (tdNodes[0].InnerText == city)
                            {
                                this.logger.LogWarning("tdNodes[0].InnerText == cityname");
                                return tdNodes;
                            }
                        }
                    }
                }
            }
            this.logger.LogWarning("GetCityNode return null");
            return null;
        }

        private OpenWeather GetWeather(string cityname)
        {
            this.logger.LogWarning("GetWeather starts");
            HtmlNode main = GetMainNode();
            if(main != null)
            {
                this.logger.LogWarning("main != null");
                HtmlNodeCollection collection = GetCityNode(main, cityname);
                if(collection != null)
                {
                    this.logger.LogWarning("collection != null");
                    OpenWeather model = new OpenWeather();
                    model.City = collection[0].InnerText;
                    model.IconTitle = collection[1].InnerText;
                    HtmlNodeCollection imageCollection = collection[1].SelectNodes("img");
                    if (imageCollection != null)
                    {
                        model.IconURL = url + imageCollection[0].Attributes["src"].Value;
                    }
                    var Temperature = Regex.Match(collection[3].InnerText, @"\d+");
                    model.Temperature = System.Convert.ToDecimal(Temperature.Value);
                    var Wind = Regex.Match(collection[9].InnerText, @"\d+");
                    model.WindSpeed = System.Convert.ToDecimal(Wind.Value??"0");
                    this.logger.LogWarning("GetWeather stops");
                    return model;
                }
                this.logger.LogWarning("return null");
                return null;
            }
            this.logger.LogWarning("return null");
            return null;
        }

        public async Task<OpenWeather> GetHTTPWeather(string city)
        {
            this.logger.LogWarning("GetHTTPWeather is called");
            return await Task.Run(()=> GetWeather(city));
        }
    }
}
