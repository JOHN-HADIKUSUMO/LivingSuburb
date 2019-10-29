using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class WeatherCoordinate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LinkId { get; set; }
        public string Label { get; set; }
        public string URL { get; set; }
        public bool? Enabled { get; set; }
    }
}
