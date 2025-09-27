using Microsoft.AspNetCore.Mvc;

namespace Shopping_Web.Controllers
{
    public class DesignController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
