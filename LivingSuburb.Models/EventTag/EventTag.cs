using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class EventTag
    {
        public int EventId { get; set; }
        public int TagId { get; set; }
        public Event Event { get; set; }
        public Tag Tag { get; set; }
    }
}
