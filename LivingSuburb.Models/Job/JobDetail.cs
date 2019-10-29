using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class JobDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Company { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string State { get; set; }
        public string Suburb { get; set; }
        public string FullDescription { get; set; }
        public string Url { get; set; }
        public string StrPublishedDate { get; set; }
        public string StrClosingDate { get; set; }
    }
}
