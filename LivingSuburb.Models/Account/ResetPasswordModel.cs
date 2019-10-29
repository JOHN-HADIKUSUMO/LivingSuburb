using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LivingSuburb.Models.Account
{
    public class ResetPasswordModel
    {
        [HiddenInput]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name ="Confirming Password")]
        [DataType(DataType.Password)]
        public string ConPassword { get; set; }
    }
}
