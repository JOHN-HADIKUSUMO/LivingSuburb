using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class PreciousMetal:PreciousMetalModel
    {
        public PreciousMetal():base()
        {

        }

        public PreciousMetal(PreciousMetalModel model) : base()
        {
            Gold = model.Gold;
            Silver = model.Silver;
            Platinum = model.Platinum;
            Palladium = model.Palladium;
            LastUpdate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PreciousMetalId { get; set; }
    }
}
