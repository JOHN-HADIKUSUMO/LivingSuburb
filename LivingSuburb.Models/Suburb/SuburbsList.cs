using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SuburbsList
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
    }
}
