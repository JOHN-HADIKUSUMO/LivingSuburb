using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class EventEdit:EventAdd
    {
        public EventEdit():base()
        {
            EditTags = new List<SimpleItem>() { };
        }

        public int EventId { get; set; }
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
        public string DatePublishedStr { get; set; }
        public List<SimpleItem> EditTags { get; set; }
    }
}
