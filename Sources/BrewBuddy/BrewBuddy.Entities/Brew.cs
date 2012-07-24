using System;
using System.ComponentModel.DataAnnotations;
using BrewBuddy.Entities.Constants;

namespace BrewBuddy.Entities
{
    public class Brew
        : IEntity
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }

        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(4000)]
        public string Annotations { get; set; }

        public bool IsPublic { get; set; }

        public DateTime LastModified { get; set; }

        // And now for some fun: EF5 does NOT support enums in .NET 4 (it does in 4.5)
        // Dirty fugly workaround: use an int backing property.
        public int StatusValue { get; set; }
        public BrewStatus Status
        {
            get { return (BrewStatus)StatusValue; }
            set { StatusValue = (int)value; }
        }

        [MaxLength(255)]
        public string SensorId { get; set; }
    }
}