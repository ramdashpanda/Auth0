using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MvcApplication.Controllers
{
    [RoutePrefix("api/Employees")]
    public class EmployeesController : ApiController
    {
        private static List<string> Item = new List<string> { "Access Token", "JWT", "OpenID" };

        [HttpGet]
        [Route("GetAll")]
        [Authorize]
        public IHttpActionResult Get()
        {

            return Ok(Item);
        }
    }
}
