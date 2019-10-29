using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class UserId
    {
        public UserId()
        {
            Id = string.Empty;
        }

        public string Id { get; set; }
    }
}
