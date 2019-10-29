using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LivingSuburb.Models.Account
{
    public class ForgotPassword
    {
        [Required]
        public string Email { get; set; }
    }
}
