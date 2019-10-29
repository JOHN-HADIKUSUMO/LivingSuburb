using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class SearchSuburbsResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameURL { get; set; }
        public string State { get; set; }
    }
}
