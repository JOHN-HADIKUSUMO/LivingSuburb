using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class JobTag
    {
        public int JobId { get; set; }
        public int TagId { get; set; }
    }
}
