using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BrewBuddy.Services;
using BrewBuddy.Web.ViewModels;

namespace BrewBuddy.Web.Api.v1
{
    [Authorize]
    public class RecipesController
        : ApiController
    {
        protected IUserService UserService { get; private set; }
        protected IRecipeService RecipeService { get; private set; }

        public RecipesController(IUserService userService, IRecipeService recipeService)
        {
            UserService = userService;
            RecipeService = recipeService;
        }

        public IQueryable<RecipeViewModel> Get()
        {
            var recipes = RecipeService.GetRecipes(User.Identity.Name);
            var model = AutoMapper.Mapper.Map(recipes, new List<RecipeViewModel>());

            return model.AsQueryable();
        }
    }
}