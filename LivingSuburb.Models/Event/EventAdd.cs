using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class EventAdd
    {
        public EventAdd()
        {
            DeletedTags = new List<SimpleItem>(){};
            NewTags = new List<SimpleItem>(){};
        }

        public string Title { get; set; }
        public string TitleURL { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public SimpleItem Country { get; set; }
        public string Location { get; set; }
        public SimpleItem Category { get; set; }
        public SimpleItem EventType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime DatePublished { get; set; }
        public string Url { get; set; }
        public List<SimpleItem> DeletedTags { get; set; }
        public List<SimpleItem> NewTags { get; set; }
    }
}
