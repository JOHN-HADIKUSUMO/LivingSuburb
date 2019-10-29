using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LivingSuburb.Models.Account
{
    public class TokenStore
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
