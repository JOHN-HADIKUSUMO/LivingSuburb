using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class JobEdit
    {
        public JobEdit()
        {
            DeletedTags = new List<SimpleItem>(){};
            NewTags = new List<SimpleItem>(){};
            EditTags = new List<SimpleItem>(){};
        }

        public int JobId { get; set; }
        public string Title { get; set; }
        public string TitleURL { get; set; }
        public string Code { get; set; }
        public string Company { get; set; }
        public SimpleItem Category { get; set; }
        public SimpleItem SubCategory { get; set; }
        public SimpleItem State { get; set; }
        public SimpleItem Suburb { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Url { get; set; }
        public string PublishingDate { get; set; }
        public string ClosingDate { get; set; }
        public List<SimpleItem> DeletedTags { get; set; }
        public List<SimpleItem> NewTags { get; set; }
        public List<SimpleItem> EditTags { get; set; }
    }
}
