using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BrewBuddy.Services;
using BrewBuddy.Web.ViewModels;

namespace BrewBuddy.Web.Controllers
{
    [Authorize]
    public class MyRecipesController
        : Controller
    {
        protected IUserService UserService { get; private set; }
        protected IRecipeService RecipeService { get; private set; }

        public MyRecipesController(IUserService userService, IRecipeService recipeService)
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
            var recipes = RecipeService.GetRecipes(User.Identity.Name);
            var model = AutoMapper.Mapper.Map(recipes, new List<RecipeViewModel>());

            return View(model);
        }

        public ActionResult Create()
        {
            return View(new RecipeViewModel());
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Create_Post(RecipeViewModel postedModel)
        {
            if (ModelState.IsValid)
            {
                var recipe = RecipeService.CreateRecipe(User.Identity.Name, postedModel.Title, postedModel.Description, postedModel.Ingredients, postedModel.Instructions);

                if (postedModel.IsPublic)
                {
                    RecipeService.MakePublic(recipe.Id);
                }
                else
                {
                    RecipeService.MakePrivate(recipe.Id);
                }

                return RedirectToAction("Details", new { id = recipe.Id });
            }

            return View(postedModel);
        }

        public ActionResult Clone(int id)
        {
            var originalId = id;
            var recipe = RecipeService.GetRecipeAsCopy(id);
            var model = AutoMapper.Mapper.Map(recipe, new RecipeViewModel());

            model.Description = string.Format("> This recipe is a copy of [{0}]({1})\r\n\r\n{2}", recipe.Title, Url.Action("Details", "Recipes", new { id = originalId }), recipe.Description);

            return View("Create", model);
        }

        public ActionResult Details(int id)
        {
            var recipe = RecipeService.GetRecipe(id);
            var model = AutoMapper.Mapper.Map(recipe, new RecipeViewModel());

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            RecipeService.DeleteRecipe(id);
            return RedirectToAction("List");
        }

        public ActionResult Edit(int id)
        {
            var recipe = RecipeService.GetRecipe(id);
            var model = AutoMapper.Mapper.Map(recipe, new RecipeViewModel());

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult Edit_Post(RecipeViewModel postedModel)
        {
            if (ModelState.IsValid)
            {
                RecipeService.UpdateRecipe(User.Identity.Name, postedModel.Id, postedModel.Title, postedModel.Description, postedModel.Ingredients, postedModel.Instructions);

                if (postedModel.IsPublic)
                {
                    RecipeService.MakePublic(postedModel.Id);
                }
                else
                {
                    RecipeService.MakePrivate(postedModel.Id);
                }

                return RedirectToAction("Details", new { id = postedModel.Id });
            }

            return View(postedModel);
        }
    }
}
