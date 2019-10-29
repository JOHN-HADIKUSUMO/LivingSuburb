using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class EventSimpleItem2 : SimpleItem2
    {
        public EventSimpleItem2():base()
        {

        }

        public EventSimpleItem2(int id,string title,string titleURL,string shortdescription, string category, string categoryURL, string eventtype, string eventtypeURL, string period,string country) :base(id, title)
        {
            TitleURL = titleURL;
            ShortDescription = shortdescription;
            Category = category;
            CategoryURL = categoryURL;
            EventType = eventtype;
            EventTypeURL = eventtypeURL;
            Period = period;
            Country = country;
        }
        public string TitleURL { get; set; }
        public string ShortDescription { get; set; }
        public string Category { get; set; }
        public string CategoryURL { get; set; }
        public string EventType { get; set; }
        public string EventTypeURL { get; set; }
        public string Period { get; set; }
        public string Country { get; set; }
    }
}
