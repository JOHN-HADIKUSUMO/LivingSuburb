using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class GallerySimpleItem : SimpleItem
    {
        public GallerySimpleItem():base()
        {

        }

        public GallerySimpleItem(int id,string name,string url, string url_114_64,string url_570_320) :base(id,name)
        {
            URL = url;
            URL_114_64 = url_114_64;
            URL_570_320 = url_570_320;
        }
        public string URL { get; set; }
        public string URL_114_64 { get; set; }
        public string URL_570_320 { get; set; }

    }
}
