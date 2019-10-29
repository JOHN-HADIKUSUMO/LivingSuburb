using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class Suburb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SuburbId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string NameURL { get; set; }
        public int Population { get; set; }
        public DateTime? Established { get; set; }
        [Required]
        public int StateId { get; set; }
        public State State { get; set; }
        public List<PostCode> PostCodes { get; set; }
    }
}
