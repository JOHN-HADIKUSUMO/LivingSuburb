using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class JobSubCategorySearch : SearchKeywords
    {
        public int Category { get; set; }
        public int OrderBy { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int BlockSize { get; set; }
    }
}
