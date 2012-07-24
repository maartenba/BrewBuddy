using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrewBuddy.Services;
using BrewBuddy.Web.ViewModels;

namespace BrewBuddy.Web.Controllers
{
    public class RecipesController
        : Controller
    {
        protected IUserService UserService { get; private set; }
        protected IRecipeService RecipeService { get; private set; }

        public RecipesController(IUserService userService, IRecipeService recipeService)
        {
            UserService = userService;
            RecipeService = recipeService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var recipes = RecipeService.GetPublicRecipes();
            var model = AutoMapper.Mapper.Map(recipes, new List<RecipeViewModel>());

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var recipe = RecipeService.GetPublicRecipe(id);
            var model = AutoMapper.Mapper.Map(recipe, new RecipeViewModel());

            return View(model);
        }
    }
}
