using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class News
    {
        public News()
        {
            NewsTitle = string.Empty;
            NewsURL = string.Empty;
            NewsCategory = string.Empty;
            NewsSource = string.Empty;
            NewsCountry = string.Empty;
            LastUpdate = DateTime.Now;
            DatePublished = DateTime.MaxValue;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NewsId { get; set; }
        public string NewsTitle { get; set; }
        public string NewsURL { get; set; }
        public string NewsCategory { get; set; }
        public string NewsSource { get; set; }
        public string NewsCountry { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
