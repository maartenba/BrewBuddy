using System;
using System.ComponentModel.DataAnnotations;

namespace BrewBuddy.Entities
{
    public class Recipe
        : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        [MaxLength(4000)]
        public string Ingredients { get; set; }

        [MaxLength(4000)]
        public string Instructions { get; set; }

        public bool IsPublic { get; set; }

        public DateTime LastModified { get; set; }
    }
}