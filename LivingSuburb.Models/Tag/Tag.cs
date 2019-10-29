using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class Tag
    {
        public Tag()
        {
            TagGroupId = (int)TagGroupType.Job_Search;
            Name = string.Empty;
        }

        public Tag(TagGroupType tagGroupType,string name)
        {
            TagGroupId = (int)tagGroupType;
            Name = name;
        }

        public Tag(int tagGroupId, string name)
        {
            TagGroupId = tagGroupId;
            Name = name;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TagId { get; set; }
        [Required]
        public int TagGroupId { get; set; }
        [Required]
        public string Name { get; set; }
        public TagGroup TagGroup { get; set; }

    }
}
