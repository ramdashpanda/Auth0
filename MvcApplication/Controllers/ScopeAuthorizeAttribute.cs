using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace MvcApplication.Controllers
{
    public class ScopeAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string permission;

        public ScopeAuthorizeAttribute(string permission)
        {
            this.permission = permission;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            // Get the Auth0 domain, in order to validate the issuer
            var domain = $"https://{ConfigurationManager.AppSettings["auth0:Domain"]}/";

            // Get the claim principal
            ClaimsPrincipal principal = actionContext.ControllerContext.RequestContext.Principal as ClaimsPrincipal;

            if(principal?.Claims.FirstOrDefault() == null)
            {
                HandleUnauthorizedRequest(actionContext);
            }
            else
            {
                // Get the scope clain. Ensure that the issuer is for the correcr Auth0 domain
                var scopeClaim = principal?.Claims.FirstOrDefault(c => c.Type == "scope" && c.Issuer == domain);
                if (scopeClaim != null)
                {
                    // Split scopes
                    var scopes = scopeClaim.Value.Split(' ');

                    // Succeed if the scope array contains the required scope
                    //if (scopes.Any(s => s == scope))
                    //    return;
                }

                // First check for permissions, they may show up in addition to or instead of scopes...
                var permissions = principal?.Claims.Where(c => c.Type == "permissions" && c.Issuer == domain)
                                                   .Select(c =>c.Value)
                                                   .ToList();
                if (permissions.Any(s => s == permission))
                    return;
                else
                    HandleUnauthorizedRequest(actionContext);
            }

        }

        //protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        //{
        //    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
        //}
    }
}