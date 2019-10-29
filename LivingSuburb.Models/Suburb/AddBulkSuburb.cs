using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class AddBulkSuburb
    {
        public string names { get; set; }
        public int stateid { get; set; }
        public string established { get; set; }
    }
}
