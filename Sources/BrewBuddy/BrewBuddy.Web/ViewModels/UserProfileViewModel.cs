using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BrewBuddy.Web.ViewModels
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "We need your e-mail address."), DataType(DataType.EmailAddress), Display(Name = "E-mail address (used to show a Gravatar, we don't show your e-mail to others)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "We want to know your name.")]
        public string Name { get; set; }

        public string Location { get; set; }

        public string Information { get; set; }

        public bool IsSelf { get; set; }
    }
}