using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class OpenWeatherCoordinate
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal lon { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal lat { get; set; }
    }
}
