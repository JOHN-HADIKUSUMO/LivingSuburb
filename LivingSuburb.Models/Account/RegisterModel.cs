using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace LivingSuburb.Models.Account
{
    public class RegisterModel : InputModel
    {
        public RegisterModel()
        {
            ExternalLogins = new List<AuthenticationScheme>() { };
            ReturnUrl = string.Empty;
        }
        public RegisterModel(InputModel model):base()
        {
            ExternalLogins = new List<AuthenticationScheme>() { };
            ReturnUrl = string.Empty;
        }
        [Required]
        [Display(Name = "Confirming Password")]
        [DataType(DataType.Password)]
        public string ConPassword { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Fullname { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }
    }
}
