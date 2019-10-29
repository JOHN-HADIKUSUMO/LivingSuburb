using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class JobSimple
    {
        public JobSimple()
        {

        }

        public JobSimple(int id,string title,string titleURL,string description,string company,string state,string suburb,string url,string category,string subcategory,string strPublishingDate, DateTime publishingDate)
        {
            Id = id;
            Title = title;
            TitleURL = titleURL;
            Description = description;
            Company = company;
            State = state;
            Suburb = suburb;
            URL = url;
            Category = category;
            SubCategory = subcategory;
            StrPublishingDate = strPublishingDate;
            PublishingDate = publishingDate;
        }

        public JobSimple(int id, string title, string titleURL, string description, string company, string state, string suburb, string url, string category, string subcategory, string strPublishingDate, DateTime publishingDate,bool isApproved, bool isExpired)
        {
            Id = id;
            Title = title;
            TitleURL = titleURL;
            Description = description;
            Company = company;
            State = state;
            Suburb = suburb;
            URL = url;
            Category = category;
            SubCategory = subcategory;
            StrPublishingDate = strPublishingDate;
            PublishingDate = publishingDate;
            IsApproved = isApproved;
            IsExpired = isExpired;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleURL { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string State { get; set; }
        public string Suburb { get; set; }
        public string URL { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string StrPublishingDate { get; set; }
        public DateTime PublishingDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsExpired { get; set; }
    }
}
