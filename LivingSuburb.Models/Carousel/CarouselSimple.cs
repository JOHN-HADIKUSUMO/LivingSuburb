using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class CarouselSimple
    {
        public CarouselSimple()
        {

        }

        public CarouselSimple(int id,string title,DateTime? datePublished)
        {
            Id = id;
            Title = title;
            if(datePublished != null)
                PublishingDate = string.Format("{0:yyyy/MM/dd}", datePublished);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string PublishingDate { get; set; }
    }
}
