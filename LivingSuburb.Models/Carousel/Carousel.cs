using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class Carousel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CarouselId { get; set; }
        public string ImageURL { get; set; }
        public string Location { get; set; }
        public string Proverb { get; set; }
        public string Source { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}
