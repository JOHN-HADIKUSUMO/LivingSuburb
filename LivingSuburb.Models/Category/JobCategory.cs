using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class JobCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string NameURL { get; set; }
        public int Rank { get; set; }
        public List<JobSubCategory> JobSubCategories { get; set; }
    }
}
