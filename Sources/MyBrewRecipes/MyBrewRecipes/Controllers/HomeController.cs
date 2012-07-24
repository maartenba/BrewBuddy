using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using WindowsAzure.Acs.Oauth2.Client;

namespace MyBrewRecipes.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Connect(string code, string error)
        {
            if (error == "usercancelled")
            {
                return RedirectToAction("Index");
            }


            string clientId = "06B059BE-E8AF-4FD5-A784-833A988A64A0";
            string clientSecret = "AE3A7E6E-DA37-4F99-96A9-70FFCCAACDE1";
            string redirectUri = "http://localhost:3476/Home/Connect/";



            string authorizeUri = "http://brewbuddy.azurewebsites.net/authorize";
            string scope = "http://www.brewbuddy.net/";

            var client = new SimpleOAuth2Client(
                new Uri(authorizeUri),
                new Uri("https://brewbuddy-euwest-1-sb.accesscontrol.windows.net/v2/OAuth2-13/"),
                clientId,
                clientSecret,
                scope,
                new Uri(redirectUri));

            if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(error))
            {
                return Redirect(client.BuildAuthorizationUri().ToString());
            }

            client.Authorize(code);

            HttpWebRequest webRequest = HttpWebRequest.Create(new Uri("http://brewbuddy.azurewebsites.net/api/v1/recipes")) as HttpWebRequest;
            webRequest.Method = WebRequestMethods.Http.Get;
            webRequest.ContentLength = 0;
            client.AppendAccessTokenTo(webRequest);

            var responseText = "";
            try
            {
                var response = webRequest.GetResponse();
                responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();

                var recipes = JsonConvert.DeserializeObject<List<RecipeViewModel>>(responseText);

                return View("Recipes", recipes);
            }
            catch (WebException wex)
            {
                responseText = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();
            }

            return Content(responseText);
        }
    }
}
