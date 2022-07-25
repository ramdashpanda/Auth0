using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MvcApplication.Controllers
{
    public class AuthorizeController : ApiController
    {
        
        
        [Route("api/Authorize/GetToken")]
        public IHttpActionResult GetToken(string clientId, string clientSecret)
        {
            string audienceURI = ConfigurationManager.AppSettings["auth0:ApiIdentifier1"]; //"https://api.surepayroll.com/api/v1/";
            string url = "https://" + ConfigurationManager.AppSettings["auth0:Domain"] + "/oauth/token";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&client_id=" + clientId + "&client_secret=" + clientSecret + "&audience=" + audienceURI, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return Ok(response.Content);
        }


    }
}