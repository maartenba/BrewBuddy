using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WindowsAzure.Acs.Oauth2;

namespace BrewBuddy.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Features()
        {
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }


        /// <summary>
        /// This method should not be in production. It is solely used to preconfigure the Windows Azure Access Control Service.
        /// </summary>
        /// <returns></returns>
        public ActionResult ProvisionApplication()
        {
            string clientId = "06B059BE-E8AF-4FD5-A784-833A988A64A0";
            string clientSecret = "AE3A7E6E-DA37-4F99-96A9-70FFCCAACDE1";
            string redirectUri = "http://localhost:3476/Home/Connect/";

            // Register the demo client application
            try
            {
                var x = new ApplicationRegistrationService();
                x.RegisterApplication(clientId, clientSecret, redirectUri, "MyBrewRecipes");

                // Also remove the
                x.RemoveDelegation(clientId, "maartenba", "");
            }
            catch
            {
            }

            // Remove delegation for our test user
            try
            {
                var x = new ApplicationRegistrationService();
                x.RemoveDelegation(clientId, "maartenba", "");
            }
            catch
            {
            }

            return Content("The client_id has been provisioned.");
        }
    }
}
