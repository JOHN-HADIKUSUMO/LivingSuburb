using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace LivingSuburb.Models
{
    public class PreciousMetalModel
    {
        public PreciousMetalModel()
        {
            Gold = 0.0M;
            Silver = 0.0M;
            Platinum = 0.0M;
            Palladium = 0.0M;
            LastUpdate = DateTime.MaxValue;
        }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Gold { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Silver { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Platinum { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Palladium { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
