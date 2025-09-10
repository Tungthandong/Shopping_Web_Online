using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.DataAccess;
using Shopping_Web.Models;
using Shopping_Web.Services;
using Shopping_Web.ViewModels;

namespace Shopping_Web.Controllers
{
    public class AdminController : Controller
    {
        IOrderService _orderService;
        IAccountService _accountService;
        IProductService _productService;
        ICategoryService _categoryService;
        public AdminController(IOrderService orderService, IAccountService accountService, IProductService productService, ICategoryService categoryService)
        {
            _orderService = orderService;
            _accountService = accountService;
            _productService = productService;
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (username == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else if (!role.Equals("admin"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var listOrder = _orderService.GetOrders();
            var customers = _accountService.GetAccounts();
            var bestSeller = _productService.getBestSellers();

            var report = GetMonthlyReport(listOrder, customers);
            ViewBag.recentOrder = listOrder.OrderByDescending(o => o.OrderDate).Take(5).ToList();
            ViewBag.bestSeller = bestSeller;
            return View(report);
        }
        public IActionResult Order(string? search, string? status, string? fromDate, string? toDate, int page = 1)
        {
            ViewBag.Msg = TempData["Msg"];
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (username == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else if (!role.Equals("admin"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            int pagesize = 4;
            var list = _orderService.GetOrders();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(o => o.Username.ToLower().Contains(search)).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                list = list.Where(o => o.OrderStatus == status).ToList();
            }
            if (!string.IsNullOrEmpty(fromDate))
            {
                list = list.Where(a => a.OrderDate.Date >= DateTime.Parse(fromDate).Date).ToList();
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                list = list.Where(a => a.OrderDate.Date <= DateTime.Parse(toDate).Date).ToList();
            }
            int totalpage = (int)Math.Ceiling((decimal)list.Count() / pagesize);
            var pagedOrder = list
                                .Skip((page - 1) * pagesize)
                                .Take(pagesize)
                                .ToList();
            ViewBag.Totalpage = totalpage;
            ViewBag.Pageindex = page;
            return View(pagedOrder);
        }
        public DashboardReport GetMonthlyReport(List<Order> listOrder, List<Account> customers)
        {
            int curYear = DateTime.Now.Year;
            int curMonth = DateTime.Now.Month;

            int prevYear = curMonth == 1 ? curYear - 1 : curYear;
            int prevMonth = curMonth == 1 ? 12 : curMonth - 1;

            var monthlyRevenue = listOrder
                .Where(o => o.OrderStatus == "Completed")
                .SelectMany(o => o.OrderDetails.Select(od => new
                {
                    o.OrderDate.Year,
                    o.OrderDate.Month,
                    Revenue = od.Quantity * od.Price
                }))
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Revenue = g.Sum(x => x.Revenue) })
                .ToList();

            var curRevenue = monthlyRevenue.FirstOrDefault(x => x.Year == curYear && x.Month == curMonth);
            var prevRevenue = monthlyRevenue.FirstOrDefault(x => x.Year == prevYear && x.Month == prevMonth);

            var monthlyOrders = listOrder
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToList();

            var curOrders = monthlyOrders.FirstOrDefault(x => x.Year == curYear && x.Month == curMonth);
            var prevOrders = monthlyOrders.FirstOrDefault(x => x.Year == prevYear && x.Month == prevMonth);

            var curCustomers = customers.Count(c => c.CreateDate.Year == curYear && c.CreateDate.Month == curMonth && c.Role == "customer");
            var prevCustomers = customers.Count(c => c.CreateDate.Year == prevYear && c.CreateDate.Month == prevMonth && c.Role == "customer");

            var totalOrdersCur = listOrder.Count(o => o.OrderDate.Year == curYear && o.OrderDate.Month == curMonth);
            var completedOrdersCur = listOrder.Count(o => o.OrderDate.Year == curYear && o.OrderDate.Month == curMonth && o.OrderStatus == "Completed");
            decimal curConversion = totalOrdersCur > 0 ? Math.Round((decimal)completedOrdersCur * 100 / totalOrdersCur, 2) : 0;

            var totalOrdersPrev = listOrder.Count(o => o.OrderDate.Year == prevYear && o.OrderDate.Month == prevMonth);
            var completedOrdersPrev = listOrder.Count(o => o.OrderDate.Year == prevYear && o.OrderDate.Month == prevMonth && o.OrderStatus == "Completed");
            decimal prevConversion = totalOrdersPrev > 0 ? Math.Round((decimal)completedOrdersPrev * 100 / totalOrdersPrev, 2) : 0;

            decimal conversionGrowth = (prevConversion > 0)
                ? Math.Round((curConversion - prevConversion) * 100 / prevConversion, 2)
                : 0;
            return new DashboardReport
            {
                Year = curYear,
                Month = curMonth,

                CurrentRevenue = curRevenue?.Revenue ?? 0,
                PreviousRevenue = prevRevenue?.Revenue ?? 0,
                RevenueGrowthPercent = (curRevenue != null && prevRevenue != null && prevRevenue.Revenue > 0)
                    ? Math.Round((curRevenue.Revenue - prevRevenue.Revenue) * 100m / prevRevenue.Revenue, 2) : 0,

                CurrentOrders = curOrders?.Count ?? 0,
                PreviousOrders = prevOrders?.Count ?? 0,
                OrderGrowthPercent = (curOrders != null && prevOrders != null && prevOrders.Count > 0)
                    ? Math.Round((curOrders.Count - prevOrders.Count) * 100m / prevOrders.Count, 2) : 0,

                CurrentCustomers = curCustomers,
                PreviousCustomers = prevCustomers,
                CustomerGrowthPercent = (curCustomers > 0 && prevCustomers > 0)
                    ? Math.Round((decimal)(curCustomers - prevCustomers) * 100 / prevCustomers, 2) : 0,

                CurrentConversionRate = curConversion,
                PreviousConversionRate = prevConversion,
                ConversionRateGrowthPercent = conversionGrowth
            };
        }
        public IActionResult UpdateOrderStatus(int oid, string status, string url)
        {
            string msg = _orderService.UpdateOrderStatus(oid, status);
            TempData["Msg"] = msg;
            return Redirect(url);
        }

        public IActionResult Account(string? search, string? status, string? roleAcc, string? fromDate, string? toDate, int page = 1)
        {
            ViewBag.Msg = TempData["Msg"];
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (username == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else if (!role.Equals("admin"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            int pagesize = 4;
            var list = _accountService.GetAccounts();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(a => a.Username.ToLower().Contains(search)).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                list = list.Where(a => a.AccountStatus == status).ToList();
            }
            if (!string.IsNullOrEmpty(roleAcc))
            {
                list = list.Where(a => a.Role == roleAcc).ToList();
            }
            if (!string.IsNullOrEmpty(fromDate))
            {
                list = list.Where(a => a.CreateDate.Date >= DateTime.Parse(fromDate).Date).ToList();
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                list = list.Where(a => a.CreateDate.Date <= DateTime.Parse(toDate).Date).ToList();
            }
            int totalpage = (int)Math.Ceiling((decimal)list.Count() / pagesize);
            var paged = list
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();
            ViewBag.Totalpage = totalpage;
            ViewBag.Pageindex = page;
            return View(paged);
        }

        public IActionResult UpdateAccountStatus(string username, string status, string url)
        {
            string msg = _accountService.UpdateAccountStatus(username, status);
            TempData["Msg"] = msg;
            return Redirect(url);
        }
        public IActionResult Product(int? categoryId, string? search, string? status, string? stock, int page = 1)
        {
            ViewBag.Msg = TempData["Msg"];
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (username == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else if (!role.Equals("admin"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            int pagesize = 4;
            var list = _productService.GetProducts();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(p => p.ProductName.ToLower().Contains(search.ToLower())).ToList();
            }
            if (categoryId != null)
            {
                list = list.Where(p => p.Category.CategoryId == categoryId).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                list = list.Where(p => p.ProductStatus == status).ToList();
            }
            if (!string.IsNullOrEmpty(stock))
            {
                if (stock == "in")
                {
                    list = list.Where(p => p.UnitsInStock > 0).ToList();
                }
                else
                {
                    list = list.Where(p => p.UnitsInStock == 0).ToList();
                }
            }
            int totalpage = (int)Math.Ceiling((decimal)list.Count() / pagesize);
            var paged = list
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();
            ViewBag.Totalpage = totalpage;
            ViewBag.Pageindex = page;
            ViewBag.Categories = _categoryService.getCategories();
            ViewBag.seller = _productService.getBestSellers();
            return View(paged);
        }
        [HttpPost]
        public IActionResult EditProduct(IFormCollection form, IFormFile ImageFile)
        {
            try
            {
                var categoryId = !string.IsNullOrEmpty(form["cid"]) ? (int?)int.Parse(form["cid"]) : null;
                var search = form["search"];
                var status = form["status"];
                var stock = form["stock"];
                var page = !string.IsNullOrEmpty(form["page"]) ? int.Parse(form["page"]) : 1;

                int productId;
                if (!int.TryParse(form["ProductId"], out productId))
                {
                    return RedirectToAction("Product", new { categoryId, search = search.ToString(), status = status.ToString(), stock = stock.ToString(), page });
                }

                var product = _productService.GetProducts().FirstOrDefault(p => p.ProductId == productId);
                if (product == null)
                {
                    return RedirectToAction("Product", new { categoryId, search = search.ToString(), status = status.ToString(), stock = stock.ToString(), page });
                }

                product.ProductName = form["ProductName"];
                product.UnitPrice = string.IsNullOrEmpty(form["UnitPrice"]) ? null : int.Parse(form["UnitPrice"]);
                product.UnitsInStock = string.IsNullOrEmpty(form["UnitsInStock"]) ? null : int.Parse(form["UnitsInStock"]);
                if (product.UnitsInStock==0)
                {
                    product.ProductStatus = "inactive";
                }
                else
                {
                    product.ProductStatus = form["ProductStatus"];
                }
                product.CategoryId = string.IsNullOrEmpty(form["CategoryId"]) ? null : int.Parse(form["CategoryId"]);
                product.IsFeatured = form["IsFeatured"].Contains("true");

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine("wwwroot/images/products", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }
                    product.Image = "/images/products/" + fileName;
                }

                _productService.UpdateProduct(product);

                return RedirectToAction("Product", new
                {
                    categoryId,
                    search = search.ToString(),
                    status = status.ToString(),
                    stock = stock.ToString(),
                    page
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Product");
            }
        }
        //[HttpPost]
        //public IActionResult AddProduct(Product model, List<IFormFile> Images)
        //{
        //    List<string> urls = new List<string>();

        //    foreach (var file in Images)
        //    {
        //        if (file.Length > 0)
        //        {
        //            var fileName = Path.GetFileName(file.FileName);
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }
        //            urls.Add("/images/" + fileName);
        //        }
        //    }

        //    model.Image = string.Join("|", urls);

        //    _productService.AddProduct(model);

        //    return RedirectToAction("Product");
        //}
        public IActionResult AddProduct(IFormCollection form, IFormFile ImageFile)
        {
            var product = new Product();

            product.ProductName = form["ProductName"];
            product.UnitPrice = string.IsNullOrEmpty(form["UnitPrice"]) ? null : int.Parse(form["UnitPrice"]);
            product.UnitsInStock = string.IsNullOrEmpty(form["UnitsInStock"]) ? null : int.Parse(form["UnitsInStock"]);
            product.ProductStatus = form["ProductStatus"];
            product.CategoryId = string.IsNullOrEmpty(form["CategoryId"]) ? null : int.Parse(form["CategoryId"]);
            product.IsFeatured = form["IsFeatured"].Count > 0;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images/products", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                product.Image = "/images/products/" + fileName;
            }
            _productService.AddProduct(product);
            return RedirectToAction("Product");
        }
    }
}
