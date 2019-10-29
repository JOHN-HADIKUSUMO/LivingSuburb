using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class CountrySimpleItem : SimpleItem
    {
        public CountrySimpleItem():base()
        {

        }

        public CountrySimpleItem(int id,string name, string code):base(id,name)
        {
            Code = code;
        }
        public string Code { get; set; }
    }
}
