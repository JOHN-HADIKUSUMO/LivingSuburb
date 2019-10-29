using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SearchSuburbKeywords2:SearchSuburbKeywords
    {
        public int StateId { get; set; }
    }
}
