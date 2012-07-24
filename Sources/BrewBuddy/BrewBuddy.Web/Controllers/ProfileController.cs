using System.Web.Mvc;
using BrewBuddy.Entities;
using BrewBuddy.Services;
using BrewBuddy.Web.ViewModels;

namespace BrewBuddy.Web.Controllers
{
    [Authorize]
    public class ProfileController
        : Controller
    {
        protected IUserService UserService { get; private set; }

        public ProfileController(IUserService userService)
        {
            UserService = userService;
        }

        [AllowAnonymous]
        public ActionResult Details(string username)
        {
            var profile = UserService.GetProfile(username) ??
                UserService.CreateDefaultProfile(User.Identity.Name, "");

            var model = AutoMapper.Mapper.Map(profile, new UserProfileViewModel());
            model.IsSelf = model.UserName == User.Identity.Name;

            return View(model);
        }

        public ActionResult Edit()
        {
            var profile = UserService.GetProfile(User.Identity.Name) ?? new UserProfile();
            var model = AutoMapper.Mapper.Map(profile, new UserProfileViewModel());

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult Edit_Post(UserProfileViewModel postedModel)
        {
            if (ModelState.IsValid)
            {
                UserService.UpdateProfile(User.Identity.Name, postedModel.Email, postedModel.Name, postedModel.Location, postedModel.Information);
                return RedirectToAction("Details", new { username = User.Identity.Name });
            }

            return View(postedModel);
        }
    }
}