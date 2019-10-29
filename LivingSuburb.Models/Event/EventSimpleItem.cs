using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class EventSimpleItem : SimpleItem
    {
        public EventSimpleItem():base()
        {

        }

        public EventSimpleItem(int id,string name, string category,string period) :base(id,name)
        {
            Category = category;
            Period = period;
        }
        public string Category { get; set; }
        public string Period { get; set; }
    }
}
