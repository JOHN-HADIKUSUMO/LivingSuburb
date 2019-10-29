using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class Wind
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal speed { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal deg { get; set; }
    }
}
