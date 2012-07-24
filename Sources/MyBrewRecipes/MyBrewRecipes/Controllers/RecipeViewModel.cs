using System;

namespace MyBrewRecipes.Controllers
{
    public class RecipeViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsPublic { get; set; }
    }
}