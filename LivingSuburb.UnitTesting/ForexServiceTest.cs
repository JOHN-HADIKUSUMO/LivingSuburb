using System.IO;
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
    public class ForexServiceTest : Base
    {
        private HtmlDocument document;
        private ForexService forexService;
        private ILogger<ForexService> logger;

        public ForexServiceTest():base()
        {
            document = new HtmlDocument();
            logger = GetLogger(configuration, serviceProvider);
            forexService = new ForexService(context, configuration, logger);
        }

        private Microsoft.Extensions.Logging.ILogger<ForexService> GetLogger(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            return (ILogger<ForexService>)serviceProvider.GetService(typeof(ILogger<ForexService>));
        }

        [SetUp]
        public void Setup()
        {
            
        }

        

        private HtmlNodeCollection GetTable()
        {
            string url = "https://www.x-rates.com/table/?from=AUD&amount=1";
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(url);
            HtmlNodeCollection table = htmlDocument.DocumentNode.SelectNodes("//table[@class='tablesorter ratesTable']/tbody");
            return table;
        }
        private List<ForexTemp> GetCurrencies()
        {
            List<ForexTemp> currencies = new List<ForexTemp>();
            currencies.Add(new ForexTemp("USD", "US Dollar", 0.0M));
            currencies.Add(new ForexTemp("NZD", "New Zealand Dollar", 0.0M));
            currencies.Add(new ForexTemp("JPY", "Japanese Yen", 0.0M));
            currencies.Add(new ForexTemp("MYR", "Malaysian Ringgit", 0.0M));
            currencies.Add(new ForexTemp("IDR","Indonesian Rupiah", 0.0M));
            currencies.Add(new ForexTemp("GBD", "British Pound", 0.0M));
            currencies.Add(new ForexTemp("EUR", "Euro", 0.0M));
            currencies.Add(new ForexTemp("CAD", "Canadian Dollar", 0.0M));
            currencies.Add(new ForexTemp("SGD", "Singapore Dollar", 0.0M));
            currencies.Add(new ForexTemp("CNY", "Chinese Yuan Renminbi", 0.0M));
            return currencies;
        }

        [TestCase]
        public void AUD_USD_Test()
        {
            List<ForexTemp> currencyList = GetCurrencies();
            HtmlNodeCollection table = GetTable();
            foreach(HtmlNode node in table)
            {
                HtmlNodeCollection trs = node.SelectNodes("tr");
                foreach(HtmlNode nodeTRs in trs)
                {
                    HtmlNodeCollection tds = nodeTRs.SelectNodes("td");
                    if(tds.Count == 3)
                    {
                        foreach(ForexTemp forexTemp in currencyList)
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
            Assert.IsTrue(true);
        }

        [TestCase]
        public void GetHTTPForex_Test()
        {
            ForexModel model = forexService.GetHTTPForex();
            Assert.IsTrue(true);
        }
    }
}