using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class EventSearch2 : EventSearch
    {
        public int CategoryId { get; set; }
        public int EventTypeId { get; set; }
    }
}
