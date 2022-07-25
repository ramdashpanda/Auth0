using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebApplication2
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
          
            base.HandleUnauthorizedRequest(actionContext);

            if (actionContext.RequestContext.Principal != null &&
                actionContext.RequestContext.Principal.Identity.IsAuthenticated &&
                Roles != null)
            {
                if (!Roles.Split(',').Any(x =>
                    actionContext.RequestContext.Principal.IsInRole(x.Trim())))
                {
                    actionContext.Response.StatusCode = HttpStatusCode.Forbidden;
                }
            }
        }
    }
}