using System;
using System.ComponentModel.DataAnnotations;

namespace BrewBuddy.Web.ViewModels
{
    public class RecipeViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Ingredients")]
        public string Ingredients { get; set; }

        [Display(Name = "Instructions")]
        public string Instructions { get; set; }

        [Display(Name = "Last updated on")]
        public DateTime LastModified { get; set; }

        [Display(Name = "Publish in public recipe gallery?")]
        public bool IsPublic { get; set; }
    }
}