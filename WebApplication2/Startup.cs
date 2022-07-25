using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Jwt;
using Owin;
using WebApplication2.Support;

[assembly: OwinStartup(typeof(WebApplication2.Startup))]

namespace WebApplication2
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var domain = $"https://{ConfigurationManager.AppSettings["auth0:Domain"]}/";
            string auth0ApiIdentifier = ConfigurationManager.AppSettings["auth0:ApiIdentifier"];
            var keyResolver = new OpenIdConnectSigningKeyResolver(domain);
            HttpConfiguration httpConfiguration = new HttpConfiguration();

            app.UseJwtBearerAuthentication(
               new JwtBearerAuthenticationOptions
               {
                   AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                   //IssuerSecurityKeyProviders = 
                   TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidAudience = auth0ApiIdentifier,
                       ValidIssuer = domain,
                       IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => keyResolver.GetSigningKey(kid),
                   }
               }

           );
            app.UseCors(CorsOptions.AllowAll);
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //WebApiConfig.Register(httpConfiguration);
            app.UseWebApi(httpConfiguration);           
        }
    }
}
