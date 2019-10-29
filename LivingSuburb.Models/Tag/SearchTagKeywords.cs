using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SearchTagKeywords:SearchKeywords
    {
        public int GroupId { get; set; }
        public int Take { get; set; }
    }
}
