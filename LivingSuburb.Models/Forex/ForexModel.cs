using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class ForexModel
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal USD { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal GBP { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal NZD { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal EUR { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal JPY { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CAD { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MYR { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SGD { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal IDR { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CNY { get; set; }
    }
}
