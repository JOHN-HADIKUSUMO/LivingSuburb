using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSuburb.Models
{
    public class UserSimpleItem
    {
        public UserSimpleItem()
        {

        }

        public UserSimpleItem(string id,string fullname, string email,string imageURL, string role, bool emailConfirmed)
        {
            Id = id;
            Fullname = fullname;
            Email = email;
            ImageURL = imageURL;
            Role = role;
            EmailConfirmed = emailConfirmed;
        }
        public string Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string ImageURL { get; set; }
        public string Role { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
