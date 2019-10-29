using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class Forex:ForexModel
    {
        public Forex()
        {
            USD = 0.0M;
            GBP = 0.0M;
            NZD = 0.0M;
            EUR = 0.0M;
            JPY = 0.0M;
            CAD = 0.0M;
            MYR = 0.0M;
            SGD = 0.0M;
            IDR = 0.0M;
            CNY = 0.0M;
            LastUpdate = DateTime.Now;
        }

        public Forex(ForexModel model)
        {
            USD = model.USD;
            GBP = model.GBP;
            NZD = model.NZD;
            EUR = model.EUR;
            JPY = model.JPY;
            CAD = model.CAD;
            MYR = model.MYR;
            SGD = model.SGD;
            IDR = model.IDR;
            CNY = model.CNY;
            LastUpdate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ForexId { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
