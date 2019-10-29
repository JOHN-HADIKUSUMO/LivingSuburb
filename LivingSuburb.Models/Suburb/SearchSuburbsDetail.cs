using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SearchSuburbsDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
    }
}
