using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class EventTypeCategory
    {
        public int EventTypeId { get; set; }
        public int EventCategoryId { get; set; }
    }
}
