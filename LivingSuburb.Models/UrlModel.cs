using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class URLModel
    {
        public string GUID { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
    }
}
