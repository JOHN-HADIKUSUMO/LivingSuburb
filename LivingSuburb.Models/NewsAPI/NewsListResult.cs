using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class NewsListResult
    {
        public NewsListResult()
        {
            News = new List<List<NewsSimpleItem>>() { };
            Pages = new List<int>() { };
            SelectedPageNo = 0;
            First = null;
            Prev = null;
            Next = null;
            Last = null;
            NumberOfPages = 0;
        }
        public List<List<NewsSimpleItem>> News { get; set; }
        public List<int> Pages { get; set; }
        public int SelectedPageNo { get; set; }
        public int? First { get; set; }
        public int? Prev { get; set; }
        public int? Next { get; set; }
        public int? Last { get; set; }
        public int NumberOfPages { get; set; }
    }
}
