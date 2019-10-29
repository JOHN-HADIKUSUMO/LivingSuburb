using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class Main
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal temp { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public int temp_min { get; set; }
        public int temp_max { get; set; }
    }
}
