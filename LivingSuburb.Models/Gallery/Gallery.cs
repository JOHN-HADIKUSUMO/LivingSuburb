using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class Gallery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GalleryId { get; set; }
        [Required]
        public string Filename { get; set; }
        [Required]
        public string GUID { get; set; }
        [Required]
        public string URL { get; set; }
        public string Suburb { get; set; }
        [Required]
        public int State { get; set; }
        public string Note { get; set; }
    }
}
