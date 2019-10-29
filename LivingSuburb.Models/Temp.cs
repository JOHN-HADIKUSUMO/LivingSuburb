using System;

namespace LivingSuburb.Models
{
    public class Temp
    {
        public Temp()
        {
            CreatedDate = DateTime.Now;
        }

        public Temp(string id, string content)
        {
            Id = id;
            Content = content;
            CreatedDate = DateTime.Now;
        }
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

