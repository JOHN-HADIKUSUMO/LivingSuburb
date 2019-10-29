using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SearchCountryResult:SimpleItem
    {
        public SearchCountryResult():base()
        {
            Id = 0;
            Name = string.Empty;
        }
    }
}
