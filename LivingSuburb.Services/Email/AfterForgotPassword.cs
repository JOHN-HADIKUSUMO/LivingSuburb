using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using LivingSuburb.Models;

namespace LivingSuburb.Services.Email
{
    public class AfterForgotPassword : EmailBase
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        private StringBuilder content;
        public AfterForgotPassword(
            TemplateService templateService,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment) :
            base(
                templateService,
                configuration,
                hostingEnvironment)
        {
            Template template = templateService.Read("AfterForgotPassword");
            content = new StringBuilder(template.Content);
            Subject = template.Subject;
        }

        public override void Send()
        {
            base.Send();
            FromAddress = new MailAddress("noreply@livingsuburb.com.au", "NoReply");
            ToAddress = new MailAddress(Email, Fullname);
            Message = new MailMessage();
            Message.Subject = Subject;
            Message.From = FromAddress;
            Message.To.Add(ToAddress);
            Message.Priority = MailPriority.High;
            Message.IsBodyHtml = true;
            Message.Body = content.Replace("{{baseurl}}",BaseURL).Replace("{{fullname}}", Fullname).Replace("{{token}}", Token).ToString();
            Client.Send(Message);
        }
    }
}
