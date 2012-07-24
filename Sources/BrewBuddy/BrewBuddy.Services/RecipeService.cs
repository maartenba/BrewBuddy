using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using BrewBuddy.Entities;

namespace BrewBuddy.Services
{
    public class RecipeService
        : IRecipeService
    {
        protected IEntityRepository<Recipe> RecipeRepository { get; private set; }

        public RecipeService(IEntityRepository<Recipe> recipeRepository)
        {
            RecipeRepository = recipeRepository;
        }

        public IEnumerable<Recipe> GetRecipes(string username)
        {
            return RecipeRepository.GetAll().Where(r => r.UserName == username);
        }

        public Recipe GetRecipe(int id)
        {
            return RecipeRepository.Get(id);
        }

        public Recipe GetRecipeAsCopy(int id)
        {
            var recipe = GetRecipe(id);
            if (recipe != null)
            {
                recipe.Id = 0;
                recipe.UserName = "";
            }
            return recipe;
        }

        public IEnumerable<Recipe> GetPublicRecipes()
        {
            return RecipeRepository.GetAll().Where(r => r.IsPublic);
        }

        public Recipe GetPublicRecipe(int id)
        {
            var recipe = RecipeRepository.Get(id);
            if (recipe.IsPublic)
            {
                return recipe;
            }

            return null;
        }

        public Recipe CreateRecipe(string username, string title, string description, string ingredients,
                                   string instructions)
        {
            var recipe = new Recipe()
                             {
                                 UserName = username,
                                 Title = title,
                                 Description = description,
                                 Ingredients = ingredients,
                                 Instructions = instructions,
                                 LastModified = DateTime.UtcNow
                             };

            RecipeRepository.InsertOnCommit(recipe);
            RecipeRepository.CommitChanges();

            return recipe;
        }

        public void UpdateRecipe(string username, int id, string title, string description, string ingredients,
                                 string instructions)
        {
            var recipe = GetRecipe(id);
            if (recipe != null && recipe.UserName == username)
            {
                recipe.Title = title;
                recipe.Description = description;
                recipe.Ingredients = ingredients;
                recipe.Instructions = instructions;
                recipe.LastModified = DateTime.UtcNow;

                RecipeRepository.CommitChanges();
            }
        }

        public void DeleteRecipe(int id)
        {
            RecipeRepository.DeleteOnCommit(GetRecipe(id));

            RecipeRepository.CommitChanges();
        }

        public void MakePublic(int id)
        {
            var recipe = GetRecipe(id);
            recipe.IsPublic = true;
            recipe.LastModified = DateTime.UtcNow;
            RecipeRepository.CommitChanges();
        }

        public void MakePrivate(int id)
        {
            var recipe = GetRecipe(id);
            recipe.IsPublic = false;
            recipe.LastModified = DateTime.UtcNow;
            RecipeRepository.CommitChanges();
        }
    }
}