using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class OpenWeather
    {
        public OpenWeather():base()
        {
            LastUpdate = DateTime.Now;
        }

        public OpenWeather(string baseURL,OpenWeatherModel model) : base()
        {
            City = model.name;
            WindSpeed = model.wind.speed;
            Temperature = model.main.temp;
            IconURL = baseURL + model.weather[0].icon + ".png";
            IconTitle = model.weather[0].main;
            LastUpdate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OpenWeatherId { get; set; }
        public string City { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal WindSpeed { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Temperature { get; set; }
        public string IconURL { get; set; }
        public string IconTitle { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
