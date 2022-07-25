using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class PayrollController : Controller
    {
        // GET: Payroll
        [Authorize]
        public ActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            string access_token = claimsIdentity?.FindFirst(c => c.Type == "access_token")?.Value;
            string id_Token = claimsIdentity?.FindFirst(c => c.Type == "id_token")?.Value;

            //ViewBag.Message = "If authorized then only see this message AccessToken :- " + access_token +"ID Token:- " + id_Token;
            return View( );
        }

    }
}