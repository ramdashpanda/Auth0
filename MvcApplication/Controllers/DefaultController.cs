using System.Collections.Generic;
using System.Web.Http;

namespace MvcApplication.Controllers
{
    [RoutePrefix("api/default")]
    public class DefaultController : ApiController
    {
        [HttpGet]
        [Route("GetEmployees")]
        //[Authorize]
        public IHttpActionResult GetEmployees()
        {
            List<string> listOfEmployees = new List<string> { "test1", "test2", "test3", "test4" };
            return Ok(listOfEmployees);
        }
    }
}
