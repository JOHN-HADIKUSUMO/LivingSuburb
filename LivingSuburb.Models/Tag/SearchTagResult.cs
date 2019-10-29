using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SearchTagResult
    {
        public SearchTagResult()
        {
            Id = 0;
            Name = string.Empty;
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
