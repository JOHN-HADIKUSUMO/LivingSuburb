using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class PostCode
    {
        public int SuburbId { get; set; }
        public int Code { get; set; }
    }
}
