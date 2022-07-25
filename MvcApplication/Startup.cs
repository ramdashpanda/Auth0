using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OpenIdConnect;
using MvcApplication.Support;
using Owin;

[assembly: OwinStartup(typeof(MvcApplication.Startup))]

namespace MvcApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Auth0 parameters
            string auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
            string auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];
            string auth0ClientSecret = ConfigurationManager.AppSettings["auth0:ClientSecret"];
            string auth0RedirectUri = ConfigurationManager.AppSettings["auth0:RedirectUri"];
            string auth0PostLogoutRedirectUri = ConfigurationManager.AppSettings["auth0:PostLogoutRedirectUri"];
            var domain = $"https://{ConfigurationManager.AppSettings["auth0:Domain"]}/";
            string auth0ApiIdentifier1 = ConfigurationManager.AppSettings["auth0:ApiIdentifier1"];
            string auth0ApiIdentifier2 = ConfigurationManager.AppSettings["auth0:ApiIdentifier2"];
            var keyResolver = new OpenIdConnectSigningKeyResolver(domain);


            app.UseJwtBearerAuthentication(
                  new JwtBearerAuthenticationOptions
                  {
                      AuthenticationMode = AuthenticationMode.Active,
                      TokenValidationParameters = new TokenValidationParameters()
                      {
                          //NameClaimType = "Roles",
                          //RoleClaimType = "https://schemas.quickstarts.com/roles",
                          ValidAudience = auth0ApiIdentifier1,
                          ValidIssuer = domain,
                          IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => keyResolver.GetSigningKey(kid),
                      }
                  }
              );

            app.UseJwtBearerAuthentication(
                 new JwtBearerAuthenticationOptions
                 {
                     AuthenticationMode = AuthenticationMode.Active,
                     TokenValidationParameters = new TokenValidationParameters()
                     {

                         //NameClaimType = "Roles",
                         //RoleClaimType = "https://schemas.quickstarts.com/roles",
                         ValidAudience = auth0ApiIdentifier2,
                         ValidIssuer = domain,
                         IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => keyResolver.GetSigningKey(kid),
                     }
                 }
             );

            //WebApiConfig.Configure(app);

            HttpConfiguration config = new HttpConfiguration();
            app.Map("/api", inner =>
            {
                inner.UseWebApi(config);
            });

            // Set Cookies as default authentication type
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            //app.SetDefaultSignInAsAuthenticationType("Application");
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                //AuthenticationMode = AuthenticationMode.Passive,
                //CookieName= "FormsAuthentication.FormsCookieName",
                LoginPath = new PathString("/Account/Login"),
                //CookieSameSite = SameSiteMode.Lax,
                //More information on why the CookieManager needs to be set can be found here: 
                // https://github.com/aspnet/AspNetKatana/wiki/System.Web-response-cookie-integration-issues
                //CookieManager = new SameSiteCookieManager(new SystemWebCookieManager()),
                //Provider = provider
            });
            // Configure Auth0 authentication
           
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "Auth0",
                ResponseType = "id_token token",
                Authority = $"https://{auth0Domain}",
                ClientId = auth0ClientId,
                RedirectUri = auth0RedirectUri,
                PostLogoutRedirectUri = auth0PostLogoutRedirectUri,
                //SaveTokens = true,
                //Scope = "offline_access",
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "Roles",
                    RoleClaimType = "https://schemas.quickstarts.com/roles"
                },
            //TokenValidationParameters = new TokenValidationParameters
            //    {
            //        NameClaimType = "name"
            //    },

                // More information on why the CookieManager needs to be set can be found here: 
                // https://docs.microsoft.com/en-us/aspnet/samesite/owin-samesite
                CookieManager = new SameSiteCookieManager(new SystemWebCookieManager()),

                Notifications = new OpenIdConnectAuthenticationNotifications
                {

                    SecurityTokenValidated = notification =>
                    {
                        notification.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", notification.ProtocolMessage.IdToken));
                        notification.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", notification.ProtocolMessage.AccessToken));
                        return Task.FromResult(0);
                    },
                    //AuthorizationCodeReceived = notification =>
                    //{
                    //    notification.AuthenticationTicket.Identity.AddClaim(new Claim("authorization_code", notification.ProtocolMessage.Code));
                    //    return Task.FromResult(0);
                    //},
                    RedirectToIdentityProvider = notification =>
                    {
                        if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var logoutUri = $"https://{auth0Domain}/v2/logout?client_id={auth0ClientId}";

                            var postLogoutUri = notification.ProtocolMessage.PostLogoutRedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    // transform to absolute
                                    var request = notification.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                }
                                logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                            }

                            notification.Response.Redirect(logoutUri);
                            notification.HandleResponse();
                        }
                        return Task.FromResult(0);
                    }
                }
            });

            //app.Au
        }
    }
}