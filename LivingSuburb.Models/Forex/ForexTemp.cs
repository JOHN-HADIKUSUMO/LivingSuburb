using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class ForexTemp
    {
        public ForexTemp(string name,string longname,decimal value)
        {
            Name = name;
            LongName = longname;
            value = value;
        }
        public string Name { get; set; }
        public string LongName { get; set; }
        public decimal Value { get; set; }
    }
}
