using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class EventDetail
    {
        private StringBuilder keywords = new StringBuilder();

        public EventDetail(int id, string title, string description, string category, string eventType, string country, string location, string sourceURL,string period)
        {
            Id = id;
            Title = title;
            Description = description;
            Category = category;
            Event = eventType;
            Country = country;
            Location = location;
            SourceURL = sourceURL;
            Period = period;
            Tags = new List<string>() { };
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Event { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public string SourceURL { get; set; }
        public string Period { get; set; }
        public List<string> Tags { get; set; }
    }
}
