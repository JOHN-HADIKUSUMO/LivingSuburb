using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string TitleURL { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int EventCategoryId { get; set; }
        public int EventTypeId { get; set; }        
        [Required]
        public int CountryId { get; set; }
        public string Location { get; set; }
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
        [Required]
        public string SourceUrl { get; set; }
        [Required]
        public DateTime DatePublished { get; set; }
        public EventCategory EventCategory { get; set; }
        public EventType EventType { get; set; }
        public List<EventTag> EventTags { get; set; }
    }
}
