using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class JobSubCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobSubCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string NameURL { get; set; }
        public int Rank { get; set; }
        public int JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; }
    }
}
