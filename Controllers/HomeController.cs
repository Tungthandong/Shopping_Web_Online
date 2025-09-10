using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Models;
using Shopping_Web.Services;

namespace Shopping_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public IActionResult Index(bool back = false)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role == "admin" && !back && ControllerContext.ActionDescriptor.ControllerName != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.LatestItems = _productService.GetLatestItems();
            return View(_productService.GetFeatureItems());
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
