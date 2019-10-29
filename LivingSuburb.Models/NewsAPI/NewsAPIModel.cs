using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class NewsAPIModel
    {
        public NewsAPIModel()
        {
            status = string.Empty;
            totalResults = 0;
            articles = new List<Article>();
        }
        public string status { get; set; }
        public int totalResults { get; set; }
        public List<Article> articles { get; set; }
    }
}
