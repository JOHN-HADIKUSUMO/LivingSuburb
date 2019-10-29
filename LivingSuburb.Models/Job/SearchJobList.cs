using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SearchJobList
    {
        public string Keywords { get; set; }
        public int Category { get; set; }
        public int SubCategory { get; set; }
        public int State { get; set; }
        public string StartWith { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int BlockSize { get; set; }
    }
}
