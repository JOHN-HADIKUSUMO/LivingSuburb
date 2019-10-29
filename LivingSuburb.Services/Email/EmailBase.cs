using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace LivingSuburb.Services.Email
{
    public class EmailBase
    {
        private IConfiguration configuration;
        private string smtpHost;
        private int smtpPort;
        private string smtpUser;
        private string smtpPassword;
        private NetworkCredential credential; 
        protected SmtpClient Client;
        protected MailAddress FromAddress;
        protected MailAddress ToAddress;
        protected MailMessage Message;
        protected string Subject;
        protected string BaseURL;
        TemplateService templateService;
        public EmailBase(
            TemplateService templateService, 
            IConfiguration configuration, 
            IHostingEnvironment hostingEnvironment)
        {
            this.configuration = configuration;
            smtpHost = configuration["SMTP:Host"];
            smtpPort = int.Parse(configuration["SMTP:Port"]);
            smtpUser = configuration["SMTP:User"];
            smtpPassword = configuration["SMTP:Password"];
            credential = new NetworkCredential(smtpUser, smtpPassword);
            this.templateService = templateService;
            BaseURL = configuration["BaseURL"];
        }

        public virtual void Send()
        {
            Client = new SmtpClient();
            Client.Host = smtpHost;
            Client.Port = smtpPort;
            Client.EnableSsl = false;
            Client.Credentials = credential;
        }
    }
}
