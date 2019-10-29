using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SearchTagList
    {
        public int TagGroupId { get; set; }
        public string Keywords { get; set; }
        public string StartWith { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int BlockSize { get; set; }
    }
}
