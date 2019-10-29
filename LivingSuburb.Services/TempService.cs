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
    public class TempService
    {
        private DataContext datacontext;
        private HttpClient client;

        private readonly IConfiguration configuration;
        private readonly ILogger<PreciousMetalService> logger;

        public TempService(DataContext datacontext, IConfiguration configuration, ILogger<PreciousMetalService> logger)
        {
            this.logger = logger;
            this.logger.LogWarning("TempService Starts");
            this.datacontext = datacontext;
            this.configuration = configuration;
            this.client = new HttpClient();
            this.logger.LogWarning("TempService Stops");
        }

        public string Create(string content)
        {
            string guid = Guid.NewGuid().ToString();
            try
            {
                Temp test = this.datacontext.Temps.Where(w => w.Id == guid).FirstOrDefault();
                if (!this.datacontext.Temps.Where(w => w.Id == guid).Any())
                {
                    Temp temp = new Temp();
                    temp.Id = guid;
                    temp.Content = content;
                    this.datacontext.Temps.Add(temp);
                    this.datacontext.SaveChanges();
                    return guid;
                }
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().ToString();
            }
            return string.Empty;
        }

        public bool Create(string guid, string content)
        {
            bool status = false;
            if(!this.datacontext.Temps.Where(w=> w.Id.Equals(guid)).Any())
            {
                Temp temp = new Temp();
                temp.Id = guid;
                temp.Content = content;
                this.datacontext.Temps.Add(temp);
                this.datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public string Read(string guid)
        {
            string result = string.Empty;
            Temp temp = this.datacontext.Temps.Where(w => w.Id == guid).FirstOrDefault();
            if(temp != null)
            {
                result = temp.Content;
            }
            return result;
        }

        public bool Delete(string guid)
        {
            bool status = false;
            Temp temp = this.datacontext.Temps.Where(w => w.Id == guid).FirstOrDefault();
            if(temp != null)
            {
                this.datacontext.Temps.Remove(temp);
                this.datacontext.SaveChanges();
                status = true; ;
            }
            return status;
        }
    }
}