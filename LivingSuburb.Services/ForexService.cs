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
using HtmlAgilityPack;
using LivingSuburb.Models;
using LivingSuburb.Database;


namespace LivingSuburb.Services
{
    public class ForexService
    {
        private DataContext datacontext;
        private HttpClient client;
        private readonly IConfiguration configuration;
        private readonly ILogger<ForexService> logger;
        public ForexService(DataContext datacontext,IConfiguration configuration, ILogger<ForexService> logger)
        {
            this.logger = logger;
            this.logger.LogWarning("ForexService Starts");
            this.datacontext = datacontext;
            this.configuration = configuration;
            this.client = new HttpClient();
            this.logger.LogWarning("ForexService Stops");
        }

        private Task<bool> isEmptyTable()
        {
            this.logger.LogWarning("isEmptyTable Starts");
            bool status = this.datacontext.Forexs.Select(s => s).Any();
            this.logger.LogWarning("isEmptyTable Stops");
            return Task.FromResult<bool>(!status);
        }

        private void Create()
        {
            this.logger.LogWarning("Create Starts");
            Forex model = new Forex();
            this.datacontext.Forexs.Add(model);
            this.logger.LogWarning("Create Stops");
        }

        public async Task<bool> Update(Forex model)
        {
            this.logger.LogWarning("Update Starts");
            bool result = false;
            bool isEmpty = await isEmptyTable();
            if(isEmpty)
            {
                Create();
            }

            Forex temp = this.datacontext.Forexs.OrderBy(o=> o.ForexId).Select(s => s).FirstOrDefault();
            if(temp != null)
            {
                temp.USD = model.USD;
                temp.IDR = model.IDR;
                temp.JPY = model.JPY;
                temp.MYR = model.MYR;
                temp.NZD = model.NZD;
                temp.SGD = model.SGD;
                temp.CAD = model.CAD;
                temp.CNY = model.CNY;
                temp.EUR = model.EUR;
                temp.GBP = model.GBP;
                temp.LastUpdate = DateTime.Now;

                this.datacontext.Forexs.Update(temp);
                this.datacontext.SaveChanges();

                result = true;
            }
            this.logger.LogWarning("Update Stops");
            return result;
        }

        public async Task<Forex> Read()
        {
            this.logger.LogWarning("Read Starts");
            Forex metal = await this.datacontext.Forexs.OrderBy(o=> o.ForexId).FirstOrDefaultAsync();
            this.logger.LogWarning("Read Stops");
            return metal;
        }

        public async Task<Forex> GetForex()
        {
            return await Read();
        }

        private HtmlNodeCollection GetTable()
        {
            this.logger.LogWarning("GetTable starts at " + Environment.TickCount.ToString());
            string url = this.configuration["Forex:Url"]; 
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(url);
            HtmlNodeCollection table = htmlDocument.DocumentNode.SelectNodes("//table[@class='tablesorter ratesTable']/tbody");
            this.logger.LogWarning("GetTable stops at " + Environment.TickCount.ToString());
            return table;
        }

        private async Task<List<ForexTemp>> GetCurrenciesListAsync()
        {
            this.logger.LogWarning("GetCurrenciesList starts at " + Environment.TickCount.ToString());
            List<ForexTemp> result = await Task.Run(() => new List<ForexTemp>() {
                new ForexTemp("USD", "US Dollar", 0.0M),
                new ForexTemp("NZD", "New Zealand Dollar", 0.0M),
                new ForexTemp("JPY", "Japanese Yen", 0.0M),
                new ForexTemp("MYR", "Malaysian Ringgit", 0.0M),
                new ForexTemp("IDR", "Indonesian Rupiah", 0.0M),
                new ForexTemp("GBP", "British Pound", 0.0M),
                new ForexTemp("EUR", "Euro", 0.0M),
                new ForexTemp("CAD", "Canadian Dollar", 0.0M),
                new ForexTemp("SGD", "Singapore Dollar", 0.0M),
                new ForexTemp("CNY", "Chinese Yuan Renminbi", 0.0M)
            });
            this.logger.LogWarning("GetCurrenciesList stops at " + Environment.TickCount.ToString());
            return result;
        }

        private List<ForexTemp> GetCurrenciesList()
        {
            this.logger.LogWarning("GetCurrenciesList starts at " + Environment.TickCount.ToString());
            List<ForexTemp> currencies = new List<ForexTemp>();
            currencies.Add(new ForexTemp("USD", "US Dollar", 0.0M));
            currencies.Add(new ForexTemp("NZD", "New Zealand Dollar", 0.0M));
            currencies.Add(new ForexTemp("JPY", "Japanese Yen", 0.0M));
            currencies.Add(new ForexTemp("MYR", "Malaysian Ringgit", 0.0M));
            currencies.Add(new ForexTemp("IDR", "Indonesian Rupiah", 0.0M));
            currencies.Add(new ForexTemp("GBP", "British Pound", 0.0M));
            currencies.Add(new ForexTemp("EUR", "Euro", 0.0M));
            currencies.Add(new ForexTemp("CAD", "Canadian Dollar", 0.0M));
            currencies.Add(new ForexTemp("SGD", "Singapore Dollar", 0.0M));
            currencies.Add(new ForexTemp("CNY", "Chinese Yuan Renminbi", 0.0M));
            this.logger.LogWarning("GetCurrenciesList stops at " + Environment.TickCount.ToString());
            return currencies;
        }

        public async Task<ForexModel> GetHTTPForexAsync()
        {
            this.logger.LogWarning("GetHTTPForex starts at " + Environment.TickCount.ToString());
            ForexModel forexModel = new ForexModel();

            List<ForexTemp> currencyList = await GetCurrenciesListAsync();
            HtmlNodeCollection table = GetTable();
            foreach (HtmlNode node in table)
            {
                HtmlNodeCollection trs = node.SelectNodes("tr");
                foreach (HtmlNode nodeTRs in trs)
                {
                    HtmlNodeCollection tds = nodeTRs.SelectNodes("td");
                    if (tds.Count == 3)
                    {
                        foreach (ForexTemp forexTemp in currencyList)
                        {
                            if (tds[0].InnerText.ToUpper().Trim() == forexTemp.LongName.ToUpper().Trim())
                            {
                                HtmlNodeCollection links = tds[1].SelectNodes("a");
                                if (links.Count > 0)
                                {
                                    string val = links[0].InnerText;
                                    forexTemp.Value = decimal.Parse(val);
                                }
                            }
                        }
                    }
                }
            }

            forexModel.USD = currencyList.Where(w => w.Name == "USD").FirstOrDefault().Value;
            forexModel.SGD = currencyList.Where(w => w.Name == "SGD").FirstOrDefault().Value;
            forexModel.NZD = currencyList.Where(w => w.Name == "NZD").FirstOrDefault().Value;
            forexModel.MYR = currencyList.Where(w => w.Name == "MYR").FirstOrDefault().Value;
            forexModel.JPY = currencyList.Where(w => w.Name == "JPY").FirstOrDefault().Value;
            forexModel.IDR = currencyList.Where(w => w.Name == "IDR").FirstOrDefault().Value;
            forexModel.GBP = currencyList.Where(w => w.Name == "GBP").FirstOrDefault().Value;
            forexModel.EUR = currencyList.Where(w => w.Name == "EUR").FirstOrDefault().Value;
            forexModel.CNY = currencyList.Where(w => w.Name == "CNY").FirstOrDefault().Value;
            forexModel.CAD = currencyList.Where(w => w.Name == "CAD").FirstOrDefault().Value;

            this.logger.LogWarning("GetHTTPForex stops at " + Environment.TickCount.ToString());
            return forexModel;
        }

        public ForexModel GetHTTPForex()
        {
            this.logger.LogWarning("GetHTTPForex starts at " + Environment.TickCount.ToString());
            ForexModel forexModel = new ForexModel();

            List<ForexTemp> currencyList = GetCurrenciesList();
            HtmlNodeCollection table = GetTable();
            foreach (HtmlNode node in table)
            {
                HtmlNodeCollection trs = node.SelectNodes("tr");
                foreach (HtmlNode nodeTRs in trs)
                {
                    HtmlNodeCollection tds = nodeTRs.SelectNodes("td");
                    if (tds.Count == 3)
                    {
                        foreach (ForexTemp forexTemp in currencyList)
                        {
                            if (tds[0].InnerText.ToUpper().Trim() == forexTemp.LongName.ToUpper().Trim())
                            {
                                HtmlNodeCollection links = tds[1].SelectNodes("a");
                                if (links.Count > 0)
                                {
                                    string val = links[0].InnerText;
                                    forexTemp.Value = decimal.Parse(val);
                                }
                            }
                        }
                    }
                }
            }

            forexModel.USD = currencyList.Where(w => w.Name == "USD").FirstOrDefault().Value;
            forexModel.SGD = currencyList.Where(w => w.Name == "SGD").FirstOrDefault().Value;
            forexModel.NZD = currencyList.Where(w => w.Name == "NZD").FirstOrDefault().Value;
            forexModel.MYR = currencyList.Where(w => w.Name == "MYR").FirstOrDefault().Value;
            forexModel.JPY = currencyList.Where(w => w.Name == "JPY").FirstOrDefault().Value;
            forexModel.IDR = currencyList.Where(w => w.Name == "IDR").FirstOrDefault().Value;
            forexModel.GBP = currencyList.Where(w => w.Name == "GBP").FirstOrDefault().Value;
            forexModel.EUR = currencyList.Where(w => w.Name == "EUR").FirstOrDefault().Value;
            forexModel.CNY = currencyList.Where(w => w.Name == "CNY").FirstOrDefault().Value;
            forexModel.CAD = currencyList.Where(w => w.Name == "CAD").FirstOrDefault().Value;

            this.logger.LogWarning("GetHTTPForex stops at " + Environment.TickCount.ToString());
            return forexModel;
        }
    }
}
