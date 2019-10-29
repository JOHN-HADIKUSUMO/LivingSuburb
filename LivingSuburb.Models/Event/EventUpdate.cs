using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class EventUpdate:EventAdd
    {
        public EventUpdate()
        {
            EventId = 0;
            DeletedTags = new List<SimpleItem>() { };
            NewTags = new List<SimpleItem>() { };
        }
        public int EventId { get; set; }
    }
}
