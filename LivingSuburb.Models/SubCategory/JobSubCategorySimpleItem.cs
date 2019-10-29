using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class JobSubCategorySimpleItem:JobCategorySimpleItem
    {
        public JobSubCategorySimpleItem():base()
        {

        }

        public JobSubCategorySimpleItem(int id,string name, int rank):base(id,name,rank)
        {
            
        }
    }
}
