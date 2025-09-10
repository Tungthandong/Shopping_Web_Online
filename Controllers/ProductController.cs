using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shopping_Web.Models;
using Shopping_Web.Services;

namespace Shopping_Web.Controllers
{
    public class ProductController : Controller
    {
        IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public IActionResult Index(string? sort, decimal? minPrice, decimal? maxPrice, string? search, int? cid, int page = 1)
        {
            int pagesize = 9;
            var product = _productService.GetFilteredProducts(minPrice, maxPrice, search, cid);
            ViewBag.Cid = cid;
            ViewBag.Search = search;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            
            ViewBag.Pageindex = page;
            ViewBag.Sort = sort;
            //if (!string.IsNullOrEmpty(search))
            //{
            //    product = _productService.SearchList(search);

            //}
            //else if (minPrice != null || maxPrice != null)
            //{
            //    product = _productService.FilterByPrice(minPrice, maxPrice);

            //}
            //else
            //{
            //    product = _productService.GetProductsByCategory(cid);
            //}
            if (sort == "desc")
            {
                product = product.OrderByDescending(x => x.UnitPrice).ToList();
            }
            if (product == null || product.Count == 0)
            {
                ViewBag.Message = "No products!";
                return View();
            }

            int totalpage = (int)Math.Ceiling((decimal)product.Count / pagesize);
            var pagedProducts = product
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();
            ViewBag.Totalpage = totalpage;
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }

            return View(pagedProducts);
        }


        public IActionResult Detail(int id)
        {
            var proById = _productService.GetProductById(id);
            ViewBag.LatestItems = _productService.GetLatestItems();
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            return View(proById);
        }
    }
}
