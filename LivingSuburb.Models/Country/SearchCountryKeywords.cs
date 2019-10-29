using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SearchCountryKeywords : SearchKeywords
    {
        public int Take { get; set; }
    }
}
