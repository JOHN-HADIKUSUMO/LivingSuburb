using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class NewsSimpleItem
    {
        public NewsSimpleItem()
        {

        }

        public int Id { get; set; }
        public string Title;
        public string Category;
        public string Source;

        public NewsSimpleItem(int id,string title,string category, string source)
        {
            Id = id;
            Title = title;
            Category = category;
            Source = source;
        }
    }
}
