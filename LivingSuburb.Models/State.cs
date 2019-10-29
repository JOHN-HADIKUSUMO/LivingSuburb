using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class State
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateId { get; set; }
        [Required]
        public string ShortName { get; set; }
        [Required]
        public string LongName { get; set; }
        public List<Suburb> Suburbs { get; set; }
    }
}
