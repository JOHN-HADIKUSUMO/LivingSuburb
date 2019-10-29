using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class UpdateCarousel : CreateCarousel
    {
        public int Id { get; set; }
    }
}

