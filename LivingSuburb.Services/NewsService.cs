/* reference https://newsapi.org */
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
namespace LivingSuburb.Services
{
    public class NewsService
    {
        private DataContext datacontext;
        private HttpClient client;
        private readonly IConfiguration configuration;
        private readonly ILogger<NewsService> logger;
        public NewsService(DataContext datacontext, IConfiguration configuration, ILogger<NewsService> logger)
        {
            this.logger = logger;
            this.logger.LogWarning("NewsService - Starts");
            this.datacontext = datacontext;
            this.configuration = configuration;
            this.client = new HttpClient();
            this.logger.LogWarning("NewsService - Stops");
        }

        internal async Task<bool> isAny(string title)
        {
            this.logger.LogWarning("isAny - Starts");
            bool status = await Task.Run(() =>
            {
                return this.datacontext.News.Where(w => w.NewsTitle.Trim().ToUpper() == title.Trim().ToUpper()).Any();
            });
            this.logger.LogWarning("isAny - Stops");
            return status;
        }

        public async Task<bool> Create(News model)
        {
            bool status = false;
            this.logger.LogWarning("Create - Starts");
            if(!await isAny(model.NewsTitle))
            {
                this.datacontext.News.Add(model);
                this.datacontext.SaveChanges();
                status = true;
            }
            this.logger.LogWarning("Create - Stops");
            return status;
        }

        public async Task<List<News>> Read(string country,string category,int take)
        {
            this.logger.LogWarning("Read - Starts");
            List<News> listNews = await Task.Run(() => {
                return this.datacontext.News.Where(w => w.NewsCountry.Trim().ToUpper() == country.Trim().ToUpper() && w.NewsCategory == category).OrderByDescending(o => o.LastUpdate).ThenBy(t=> t.NewsTitle).Take(take).Select(s=> s).ToList();
            });
            this.logger.LogWarning("Read - Stops");
            return listNews;
        }

        public List<News> GetLatest(int take)
        {
            return this.datacontext.News.OrderByDescending(o => o.DatePublished).ThenBy(t => t.NewsTitle).Take(take).Select(s => s).ToList();
        }

        public async Task<NewsAPIModel> GetHTTPNews(string countryCode,string category, DateTime from,DateTime to)
        {
            StringBuilder log = new StringBuilder();
            this.logger.LogWarning("GetHTTPNews - Starts");
            this.logger.LogWarning("GetHTTPNews - countryCode = " + countryCode);
            this.logger.LogWarning("GetHTTPNews - category = " + category);
            this.logger.LogWarning("GetHTTPNews - from = " + string.Format("{0:yyyy/MM/dd hh:mm tt}",from));
            this.logger.LogWarning("GetHTTPNews - to = " + string.Format("{0:yyyy/MM/dd hh:mm tt}", to));
            NewsAPIModel news = null;
            try
            {
                string url = this.configuration["NewsAPI:Url"];
                this.logger.LogWarning("url => " + url);
                string apiKey = this.configuration["NewsAPI:ApiKey"];
                this.logger.LogWarning("apiKey => " + apiKey);
                StringBuilder tempURL = new StringBuilder(url);
                tempURL = tempURL.Replace("{{countrycode}}", countryCode).Replace("{{category}}", category).Replace("{{from}}", string.Format("{0:yyyy-MM-dd 00:00:00 AM}", from)).Replace("{{to}}", string.Format("{0:yyyy-MM-dd 59:59:59 PM}", to)).Replace("{{apikey}}", apiKey);
                this.logger.LogWarning("tempURL => " + tempURL.ToString());
                var serializer = new DataContractJsonSerializer(typeof(NewsAPIModel));
                this.logger.LogWarning("var serializer");
                Stream stream = await this.client.GetStreamAsync(tempURL.ToString());
                this.logger.LogWarning("Stream stream");
                news = (serializer.ReadObject(stream) as NewsAPIModel);
                this.logger.LogWarning("serializer.ReadObject(stream)");
            }
            catch (Exception ex) {
                this.logger.LogWarning("GetHTTPNews - Errors - " + ex.GetBaseException().ToString());
            }
            this.logger.LogWarning("GetHTTPNews - articles no = " + news.articles.Count());
            this.logger.LogWarning("GetHTTPNews - Stops");
            return news;
        }

        public NewsListResult Search(string keywords, int Orderby, int PageNo, int PageSize, int BlockSize)
        {
            NewsListResult result = new NewsListResult();
            Orderby = Orderby <= 0 ? 0 : (Orderby > 2 ? 2 : Orderby);
            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;

            if (string.IsNullOrEmpty(keywords))
            {
                switch (Orderby)
                {
                    case 0: /* by Id */
                        NumberOfRecords = datacontext.News.OrderByDescending(o => o.NewsId).Count();
                        break;
                    case 1: /* by Title */
                        NumberOfRecords = datacontext.News.OrderBy(o => o.NewsTitle).Count();
                        break;
                    case 2: /* by Source */
                        NumberOfRecords = datacontext.News.OrderBy(o => o.NewsSource).Count();
                        break;
                    default: /* by Category */
                        NumberOfRecords = datacontext.News.OrderBy(o => o.NewsCategory).Count();
                        break;
                }
            }
            else
            {
                switch (Orderby)
                {
                    case 0: /* by Id */
                        NumberOfRecords = datacontext.News.Where(w=> w.NewsTitle.StartsWith(keywords)).OrderByDescending(o => o.NewsId).Count();
                        break;
                    case 1: /* by Title */
                        NumberOfRecords = datacontext.News.Where(w => w.NewsTitle.StartsWith(keywords)).OrderBy(o => o.NewsTitle).Count();
                        break;
                    case 2: /* by Source */
                        NumberOfRecords = datacontext.News.Where(w => w.NewsTitle.StartsWith(keywords)).OrderBy(o => o.NewsSource).Count();
                        break;
                    default: /* by Category */
                        NumberOfRecords = datacontext.News.Where(w => w.NewsTitle.StartsWith(keywords)).OrderBy(o => o.NewsCategory).Count();
                        break;
                }
            }

            if (NumberOfRecords == 0)
            {
                NumberOfPages = 0;
                NumberOfBlocks = 0;
                PageNo = 0;
                BlockNo = 0;
            }
            else
            {
                if (NumberOfRecords <= PageSize)
                {
                    NumberOfPages = 1;
                    NumberOfBlocks = 1;
                    PageNo = 1;
                    BlockNo = 1;
                }
                else
                {
                    NumberOfPages = NumberOfRecords / PageSize;
                    if (NumberOfRecords % PageSize > 0)
                    {
                        NumberOfPages++;
                    }

                    NumberOfBlocks = NumberOfPages / BlockSize;
                    if (NumberOfPages % BlockSize > 0)
                    {
                        NumberOfBlocks++;
                    }

                    if (PageNo > NumberOfPages)
                    {
                        PageNo = NumberOfPages;
                    }

                    if (PageNo <= BlockSize)
                    {
                        BlockNo = 1;
                    }
                    else
                    {
                        BlockNo = (int)Math.Abs(Math.Floor((decimal)PageNo / (decimal)BlockSize));
                        if (PageNo % BlockSize > 0)
                        {
                            BlockNo++;
                        }
                    }
                }

                Start = ((BlockNo - 1) * BlockSize) + 1;
                Stop = BlockNo * BlockSize;

                if (Stop > NumberOfPages) Stop = NumberOfPages;

                for (int i = Start; i <= Stop; i++)
                {
                    result.Pages.Add(i);
                }

                if (NumberOfBlocks == 1)
                {
                    result.First = null;
                    result.Prev = null;
                    result.Next = null;
                    result.Last = null;
                }
                else
                {
                    if (BlockNo == NumberOfBlocks)
                    {
                        result.First = 1;
                        result.Prev = PageNo - 1;
                        result.Next = null;
                        result.Last = null;
                    }
                    else if (BlockNo == 1)
                    {
                        result.First = null;
                        result.Prev = null;
                        result.Next = PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                    else
                    {
                        result.First = 1;
                        result.Prev = PageNo - 1;
                        result.Next = PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                }

                int Skip = (Start - 1) * PageSize;
                int Take = (Stop - (Start - 1)) * PageSize;
                List<NewsSimpleItem> temps;

                if (string.IsNullOrEmpty(keywords))
                {
                    switch (Orderby)
                    {
                        case 0: /* by Id */
                            temps = datacontext.News.OrderByDescending(o => o.NewsId).Skip(Skip).Take(Take).Select(s=> new NewsSimpleItem(s.NewsId,s.NewsTitle,s.NewsCategory,s.NewsSource)).ToList();
                            break;
                        case 1: /* by Title */
                            temps = datacontext.News.OrderBy(o => o.NewsTitle).Skip(Skip).Take(Take).Select(s => new NewsSimpleItem(s.NewsId, s.NewsTitle, s.NewsCategory, s.NewsSource)).ToList();
                            break;
                        case 2: /* by Source */
                            temps = datacontext.News.OrderBy(o => o.NewsSource).Skip(Skip).Take(Take).Select(s => new NewsSimpleItem(s.NewsId, s.NewsTitle, s.NewsCategory, s.NewsSource)).ToList();
                            break;
                        default: /* by Category */
                            temps = datacontext.News.OrderBy(o => o.NewsCategory).Skip(Skip).Take(Take).Select(s => new NewsSimpleItem(s.NewsId, s.NewsTitle, s.NewsCategory, s.NewsSource)).ToList();
                            break;
                    }
                }
                else
                {
                    switch (Orderby)
                    {
                        case 0: /* by Id */
                            temps = datacontext.News.Where(w => w.NewsTitle.StartsWith(keywords)).OrderByDescending(o => o.NewsId).Skip(Skip).Take(Take).Select(s => new NewsSimpleItem(s.NewsId, s.NewsTitle, s.NewsCategory, s.NewsSource)).ToList();
                            break;
                        case 1: /* by Title */
                            temps = datacontext.News.Where(w => w.NewsTitle.StartsWith(keywords)).OrderBy(o => o.NewsTitle).Skip(Skip).Take(Take).Select(s => new NewsSimpleItem(s.NewsId, s.NewsTitle, s.NewsCategory, s.NewsSource)).ToList();
                            break;
                        case 2: /* by Source */
                            temps = datacontext.News.Where(w => w.NewsTitle.StartsWith(keywords)).OrderBy(o => o.NewsSource).Skip(Skip).Take(Take).Select(s => new NewsSimpleItem(s.NewsId, s.NewsTitle, s.NewsCategory, s.NewsSource)).ToList();
                            break;
                        default: /* by Category */
                            temps = datacontext.News.Where(w => w.NewsTitle.StartsWith(keywords)).OrderBy(o => o.NewsCategory).Skip(Skip).Take(Take).Select(s => new NewsSimpleItem(s.NewsId, s.NewsTitle, s.NewsCategory, s.NewsSource)).ToList();
                            break;
                    }
                }

                List<NewsSimpleItem> collection = new List<NewsSimpleItem>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.News.Add(collection);
                        collection = new List<NewsSimpleItem>();
                        count = 0;
                    }
                }
            }
            result.NumberOfPages = NumberOfPages;
            result.SelectedPageNo = PageNo;
            return result;
        }
    }
}
