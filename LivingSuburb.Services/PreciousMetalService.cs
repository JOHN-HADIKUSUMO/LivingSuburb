using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LivingSuburb.Models;
using LivingSuburb.Database;
using HtmlAgilityPack;

namespace LivingSuburb.Services
{
    public class PreciousMetalService
    {
        private DataContext datacontext;
        private HttpClient client;
        private readonly IConfiguration configuration;
        private readonly ILogger<PreciousMetalService> logger;
        public PreciousMetalService(DataContext datacontext, IConfiguration configuration, ILogger<PreciousMetalService> logger)
        {
            this.logger = logger;
            this.logger.LogWarning("PreciousMetalService Starts");
            this.datacontext = datacontext;
            this.configuration = configuration;
            this.client = new HttpClient();
            this.logger.LogWarning("PreciousMetalService Stops");
        }

        private Task<bool> isEmptyTable()
        {
            this.logger.LogWarning("isEmptyTable Starts", null);
            bool status = this.datacontext.PreciousMetals.Select(s => s).Any();
            this.logger.LogWarning("isEmptyTable Stops", null);
            return Task.FromResult<bool>(!status);
        }

        private void Create()
        {
            this.logger.LogWarning("Create Starts");
            PreciousMetal model = new PreciousMetal();
            this.datacontext.PreciousMetals.Add(model);
            this.logger.LogWarning("Create Stops");
        }

        public async Task<bool> Update(PreciousMetal model)
        {
            this.logger.LogWarning("Update Starts");
            bool result = false;
            bool isEmpty = await isEmptyTable();
            if (isEmpty)
            {
                Create();
            }

            PreciousMetal temp = this.datacontext.PreciousMetals.Select(s => s).FirstOrDefault();
            if (temp != null)
            {
                temp.Gold = model.Gold;
                temp.Silver = model.Silver;
                temp.Platinum = model.Platinum;
                temp.Palladium = model.Palladium;
                temp.LastUpdate = DateTime.Now;
                this.datacontext.PreciousMetals.Update(temp);
                this.datacontext.SaveChanges();
                result = true;
            }
            this.logger.LogWarning("Update Stops");
            return result;
        }

        public async Task<PreciousMetal> Read()
        {
            this.logger.LogWarning("Read Starts");
            PreciousMetal metal = await this.datacontext.PreciousMetals.Select(s=> s).OrderByDescending(o=>o.LastUpdate).FirstOrDefaultAsync();
            this.logger.LogWarning("metal is NULL ? = " + (metal == null));
            this.logger.LogWarning("Read Stops");
            return metal;
        }

        public async Task<PreciousMetalModel> GetPrices()
        {
            this.logger.LogWarning("GetPrices is called");
            return await Read();
        }

        public async Task<PreciousMetalModel> GetHTTPPrices()
        {
            this.logger.LogWarning("GetHTTPPrices starts at " + Environment.TickCount);
            PreciousMetalModel metal = null;
            try
            {
                var url = this.configuration["PreciousMetal:Url"];
                HtmlWeb webClient = new HtmlWeb();
                HtmlDocument document = await webClient.LoadFromWebAsync(url);
                HtmlNode goldNode = document.GetElementbyId("lblGoldAskAU");
                HtmlNode silverNode = document.GetElementbyId("lblSilverAskAU");
                HtmlNode platinumNode = document.GetElementbyId("lblPlatinumAskAU");
                HtmlNode palladiumNode = document.GetElementbyId("lblPalladiumAskAU");
                HtmlNode lastUpdateNode = document.GetElementbyId("lblDatetime");
                if (goldNode != null && silverNode != null && platinumNode != null && palladiumNode != null)
                {
                    metal = new PreciousMetalModel();
                    decimal valueGold = 0.0M;
                    if (decimal.TryParse(goldNode.InnerText.Replace("$", ""), out valueGold))
                    {
                        metal.Gold = valueGold;
                    }

                    decimal valueSilver = 0.0M;
                    if (decimal.TryParse(silverNode.InnerText.Replace("$", ""), out valueSilver))
                    {
                        metal.Silver = valueSilver;
                    }

                    decimal valuePlatinum = 0.0M;
                    if (decimal.TryParse(platinumNode.InnerText.Replace("$", ""), out valuePlatinum))
                    {
                        metal.Platinum = valuePlatinum;
                    }

                    decimal valuePalladium = 0.0M;
                    if (decimal.TryParse(palladiumNode.InnerText.Replace("$", ""), out valuePalladium))
                    {
                        metal.Palladium = valuePalladium;
                    }

                    string[] temps = lastUpdateNode.InnerText.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (temps.Length == 5)
                    {
                        string date = temps[2] + " " + temps[3] + " " + temps[4] + " " + temps[0];
                        metal.LastUpdate = DateTime.Parse(date).AddHours(3);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.GetBaseException().ToString());
                metal = null;
            }
            this.logger.LogWarning("GetHTTPPrices stops at " + Environment.TickCount);
            return metal;
        }
    }
}
