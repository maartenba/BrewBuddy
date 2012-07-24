using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using BrewBuddy.Entities;
using BrewBuddy.Entities.Constants;
using BrewBuddy.Services;
using BrewBuddy.Web.Api;
using BrewBuddy.Web.Extensions;
using BrewBuddy.Web.Filters;
using BrewBuddy.Web.ViewModels;

namespace BrewBuddy.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ProvideProfileInfoInViewBagAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "ApiV1",
                routeTemplate: "api/v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Profile",
                url: "Profile/{action}/{username}",
                defaults: new { controller = "Profile", action = "Details" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            RegisterDependencies();
            RegisterMappings();

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configuration.Filters.Add(new ValidationActionFilter());
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.EnableDefaultBundles();
            BundleTable.Bundles.EnableBootstrapBundles();
            BundleTable.Bundles.EnableRaphaelBundles();
        }

        private static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // Entities
            Database.SetInitializer<EntitiesContext>(null);

            builder.RegisterType<EntitiesContext>()
                .InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(EntityRepository<>))
                .As(typeof(IEntityRepository<>))
                .InstancePerHttpRequest();

            // Services
            builder.RegisterAssemblyTypes(typeof(IUserService).Assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            // ASP.NET stack dependencies
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterFilterProvider();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // ASP.NET Web API dependencies
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();

            var autofacDependencyResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(autofacDependencyResolver);

            var autofacWebApiDependencyResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = autofacWebApiDependencyResolver;
        }

        private void RegisterMappings()
        {
            AutoMapper.Mapper.CreateMap<UserProfile, UserProfileViewModel>();
            AutoMapper.Mapper.CreateMap<UserProfileViewModel, UserProfile>()
                .ForMember(m => m.Id, opt => opt.UseDestinationValue());

            AutoMapper.Mapper.CreateMap<Recipe, RecipeViewModel>();
            AutoMapper.Mapper.CreateMap<RecipeViewModel, Recipe>()
                .ForMember(m => m.Id, opt => opt.UseDestinationValue())
                .ForMember(m => m.UserName, opt => opt.UseDestinationValue());

            AutoMapper.Mapper.CreateMap<Brew, BrewViewModel>()
                .ForMember(m => m.Status, opt => opt.ResolveUsing(b => b.Status.ToString()));
            AutoMapper.Mapper.CreateMap<BrewViewModel, Brew>()
                .ForMember(m => m.Id, opt => opt.UseDestinationValue())
                .ForMember(m => m.UserName, opt => opt.UseDestinationValue())
                .ForMember(m => m.RecipeId, opt => opt.UseDestinationValue())
                .ForMember(m => m.SensorId, opt => opt.UseDestinationValue())
                .ForMember(m => m.Status, opt => opt.ResolveUsing(b => (BrewStatus)Enum.Parse(typeof(BrewStatus), b.Status)));
        }
    }
}