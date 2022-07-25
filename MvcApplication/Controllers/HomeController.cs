using System.Security.Claims;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            string access_token = claimsIdentity?.FindFirst(c => c.Type == "access_token")?.Value;

            return View();
        }
    }
}