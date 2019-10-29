using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string TitleURL { get; set; }
        public string Code { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public int Term { get; set; }
        [Required]
        public int Category { get; set; }
        public int? SubCategory { get; set; }
        [Required]
        public int State { get; set; }
        public int? Suburb { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string FullDescription { get; set; }
        public string Url { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
