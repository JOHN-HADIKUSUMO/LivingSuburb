using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SimpleItem2
    {
        public SimpleItem2()
        {

        }

        public SimpleItem2(int id,string title)
        {
            Id = id;
            Title = title;
        }
        public int? Id { get; set; }
        public string Title { get; set; }
    }
}
