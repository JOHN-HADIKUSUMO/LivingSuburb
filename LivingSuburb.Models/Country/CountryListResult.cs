using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class CountryListResult
    {
        public CountryListResult()
        {
            Countries = new List<List<CountrySimpleItem>>() { };
            Pages = new List<int>() { };
            SelectedPageNo = 0;
            SelectedIndex = 0;
            First = null;
            Prev = null;
            Next = null;
            Last = null;
            NumberOfPages = 0;
        }
        public List<List<CountrySimpleItem>> Countries { get; set; }
        public List<int> Pages { get; set; }
        public int SelectedPageNo { get; set; }
        public int SelectedIndex { get; set; }
        public int? First { get; set; }
        public int? Prev { get; set; }
        public int? Next { get; set; }
        public int? Last { get; set; }
        public int NumberOfPages { get; set; }
    }
}
