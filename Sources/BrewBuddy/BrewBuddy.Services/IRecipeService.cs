using System.Collections.Generic;
using BrewBuddy.Entities;

namespace BrewBuddy.Services
{
    public interface IRecipeService
    {
        IEnumerable<Recipe> GetRecipes(string username);
        Recipe GetRecipe(int id);
        Recipe GetRecipeAsCopy(int id);
        IEnumerable<Recipe> GetPublicRecipes();
        Recipe GetPublicRecipe(int id);
        Recipe CreateRecipe(string username, string title, string description, string ingredients, string instructions);
        void UpdateRecipe(string username, int id, string title, string description, string ingredients, string instructions);
        void DeleteRecipe(int id);
        void MakePublic(int id);
        void MakePrivate(int id);
    }
}