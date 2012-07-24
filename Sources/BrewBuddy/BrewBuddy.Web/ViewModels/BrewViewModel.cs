using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using BrewBuddy.Entities.Constants;

namespace BrewBuddy.Web.ViewModels
{
    public class BrewViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public int RecipeId { get; set; }

        [Display(Name = "Recipe")]
        public RecipeViewModel Recipe { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Annotations")]
        public string Annotations { get; set; }

        [Display(Name = "Last updated on")]
        public DateTime LastModified { get; set; }

        [Display(Name = "Can be consulted from the public recipe gallery?")]
        public bool IsPublic { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Sensor ID")]
        public string SensorId { get; set; }

        public IEnumerable<SelectListItem> Statuses { get; set; }

        public BrewViewModel()
        {
            PopulateStatuses();
        }

        public void PopulateStatuses()
        {
            Statuses = Enum.GetNames(typeof(BrewStatus))
                .Select(e => new SelectListItem { Text = e, Value = e })
                .ToList();
        }
    }
}