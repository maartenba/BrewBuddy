using System.Web.Mvc;
using WindowsAzure.Acs.Oauth2;

namespace BrewBuddy.Web.Controllers
{
    [Authorize]
    public class AuthorizeController
        : AuthorizationServer
    {
    }
}
