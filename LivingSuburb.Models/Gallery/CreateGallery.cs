using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class CreateGallery
    {
        public string GUID { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public string Suburb { get; set; }
        public int State { get; set; }
        public string Note { get; set; }
    }
}

