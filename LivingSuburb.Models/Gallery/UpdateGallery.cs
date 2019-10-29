using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class UpdateGallery:CreateGallery
    {
        public int Id { get; set; }
    }
}

