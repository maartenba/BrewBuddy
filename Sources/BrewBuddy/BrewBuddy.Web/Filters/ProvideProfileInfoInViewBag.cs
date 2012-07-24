using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrewBuddy.Entities;
using BrewBuddy.Services;
using BrewBuddy.Web.ViewModels;

namespace BrewBuddy.Web.Filters
{
    public class ProvideProfileInfoInViewBagAttribute
        : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var userService = DependencyResolver.Current.GetService<IUserService>();
                var profile = userService.GetProfile(filterContext.HttpContext.User.Identity.Name) ?? new UserProfile();

                filterContext.Controller.ViewBag.AuthenticatedProfile = AutoMapper.Mapper.Map(profile, new UserProfileViewModel());
            } 
        }
    }
}