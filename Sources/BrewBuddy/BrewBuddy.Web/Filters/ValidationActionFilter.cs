using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BrewBuddy.Web.Filters
{
    public class ValidationActionFilter
        : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var modelState = actionContext.ModelState;
            if (!modelState.IsValid)
            {
                dynamic errors = new JsonObject();
                foreach (var key in modelState.Keys)
                {
                    var state = modelState[key];
                    if (state.Errors.Any())
                    {
                        errors[key] = state.Errors.First().ErrorMessage;
                    }
                }

                actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                actionContext.Response.Content = new ObjectContent(typeof(JsonObject), errors, new JsonMediaTypeFormatter());
            }
        }
    }
}