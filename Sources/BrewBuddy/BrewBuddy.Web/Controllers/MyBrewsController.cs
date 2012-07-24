using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BrewBuddy.Entities.Constants;
using BrewBuddy.Services;
using BrewBuddy.Web.ViewModels;

namespace BrewBuddy.Web.Controllers
{
    [Authorize]
    public class MyBrewsController
        : Controller
    {
        protected IUserService UserService { get; private set; }
        protected IRecipeService RecipeService { get; private set; }
        protected IBrewService BrewService { get; private set; }
        protected ITemperatureAggregateService TemperatureAggregateService { get; private set; }

        public MyBrewsController(IUserService userService, IRecipeService recipeService, IBrewService brewService, ITemperatureAggregateService temperatureAggregateService)
        {
            UserService = userService;
            RecipeService = recipeService;
            BrewService = brewService;
            TemperatureAggregateService = temperatureAggregateService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var brews = BrewService.GetBrews(User.Identity.Name);
            var model = AutoMapper.Mapper.Map(brews, new List<BrewViewModel>());

            return View(model);
        }

        public ActionResult Create(int recipeId)
        {
            var recipe = RecipeService.GetRecipe(recipeId);
            var model = new BrewViewModel()
                            {
                                RecipeId = recipeId,
                                Title = recipe.Title,
                                Status = "Created",
                                Annotations = string.Format("> This brew is based on the recipe [{0}]({1})", recipe.Title, Url.Action("Details", "Recipes", new { id = recipeId }))
                            };
            model.Statuses.First().Selected = true;

            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Create_Post(int recipeId, BrewViewModel postedModel)
        {
            if (ModelState.IsValid)
            {
                var brew = BrewService.CreateBrew(User.Identity.Name, recipeId, postedModel.Title, postedModel.Annotations, (BrewStatus)Enum.Parse(typeof(BrewStatus), postedModel.Status));

                if (postedModel.IsPublic)
                {
                    BrewService.MakePublic(brew.Id);
                }
                else
                {
                    BrewService.MakePrivate(brew.Id);
                }

                return RedirectToAction("Details", new { id = brew.Id });
            }

            postedModel.Statuses.First(s => s.Text == postedModel.Status).Selected = true;

            return View(postedModel);
        }

        public ActionResult Details(int id)
        {
            var brew = BrewService.GetBrew(id);
            var model = AutoMapper.Mapper.Map(brew, new BrewViewModel());

            return View(model);
        }

        //[ChildActionOnly]
        public ActionResult RetrieveTemperatureAggregateGraph(int id)
        {
            var aggregates = TemperatureAggregateService.GetForBrew(id);
            return PartialView("_RetrieveTemperatureAggregateGraph", aggregates);
        }

        public ActionResult Link(int id)
        {
            var brew = BrewService.GetBrew(id);
            var model = AutoMapper.Mapper.Map(brew, new BrewViewModel());

            return View(model);
        }

        [HttpPost, ActionName("Link")]
        public ActionResult Link_Post(int id, BrewViewModel postedModel)
        {
            int brewId = id;
            if (!string.IsNullOrEmpty(postedModel.SensorId))
            {
                BrewService.LinkSensor(brewId, postedModel.SensorId);
            }
            else
            {
                var brew = BrewService.GetBrew(id);
                BrewService.UnlinkSensor(brewId, brew.SensorId);
            }

            return RedirectToAction("Details", new { id = brewId });
        }

        public ActionResult Delete(int id)
        {
            BrewService.DeleteBrew(id);
            return RedirectToAction("List");
        }

        public ActionResult Edit(int id)
        {
            var brew = BrewService.GetBrew(id);
            var model = AutoMapper.Mapper.Map(brew, new BrewViewModel());
            model.Statuses.First(s => s.Text == model.Status).Selected = true;

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult Edit_Post(BrewViewModel postedModel)
        {
            if (ModelState.IsValid)
            {
                BrewService.UpdateBrew(User.Identity.Name, postedModel.Id, postedModel.Title, postedModel.Annotations, (BrewStatus)Enum.Parse(typeof(BrewStatus), postedModel.Status));

                if (postedModel.IsPublic)
                {
                    BrewService.MakePublic(postedModel.Id);
                }
                else
                {
                    BrewService.MakePrivate(postedModel.Id);
                }

                return RedirectToAction("Details", new { id = postedModel.Id });
            }

            postedModel.Statuses.First(s => s.Text == postedModel.Status).Selected = true;

            return View(postedModel);
        }
    }
}
