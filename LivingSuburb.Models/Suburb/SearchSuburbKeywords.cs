using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SearchSuburbKeywords : SearchKeywords
    {
        public int Take { get; set; }
    }
}
