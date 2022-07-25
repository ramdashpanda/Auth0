using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using MvcApplication.Support;
using Owin;

[assembly: OwinStartup(typeof(WebAPI.Startup))]
namespace WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var domain = $"https://{ConfigurationManager.AppSettings["auth0:Domain"]}/";
            string auth0ApiIdentifier = ConfigurationManager.AppSettings["auth0:ApiIdentifier"];
            var keyResolver = new OpenIdConnectSigningKeyResolver(domain);


           

            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //appBuilder.UseWebApi(config);

            //WebApiConfig.Register(config);

            app.UseWebApi(config);

            app.UseJwtBearerAuthentication(
                 new JwtBearerAuthenticationOptions
                 {
                     AuthenticationMode = AuthenticationMode.Active,
                     TokenValidationParameters = new TokenValidationParameters()
                     {
                         ValidAudience = auth0ApiIdentifier,
                         ValidIssuer = domain,
                         IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => keyResolver.GetSigningKey(kid),
                     }
                 }
             );
        }
    }
}
