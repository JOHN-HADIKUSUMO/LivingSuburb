using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class AddSuburb
    {
        public string name { get; set; }
        public string nameurl { get; set; }
        public int stateid { get; set; }
        public string established { get; set; }
    }
}
