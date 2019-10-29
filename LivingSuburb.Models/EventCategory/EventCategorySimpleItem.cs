using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class EventCategorySimpleItem : SimpleItem
    {
        public EventCategorySimpleItem()
        {

        }

        public EventCategorySimpleItem(int id,string name, int rank)
        {
            Id = id;
            Name = name;
            Rank = rank;
        }
        public int Rank { get; set; }
    }
}
