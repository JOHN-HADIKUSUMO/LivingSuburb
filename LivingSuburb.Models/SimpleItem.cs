using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SimpleItem
    {
        public SimpleItem()
        {

        }

        public SimpleItem(int id,string name)
        {
            Id = id;
            Name = name;
        }
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}
