using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace LivingSuburb.Models.Account
{
    public class InputAfterPostModel:InputModel
    {
        public InputAfterPostModel()
        {
            ExternalLogins = new List<AuthenticationScheme>() { };
            ReturnUrl = string.Empty;
        }
        public InputAfterPostModel(InputModel model):base()
        {
            ExternalLogins = new List<AuthenticationScheme>() { };
            ReturnUrl = string.Empty;
        }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }
    }
}
