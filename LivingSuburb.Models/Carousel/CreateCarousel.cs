using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class CreateCarousel
    {
        public string GUID { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public string Location { get; set; }
        public string Proverb { get; set; }
        public string Source { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}

