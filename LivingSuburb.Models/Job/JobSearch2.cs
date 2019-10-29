using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class JobSearch2 : JobSearch
    {
        public int OrderBy { get; set; }
    }
}
