using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping_Web.Models;
using Shopping_Web.Services;

namespace Shopping_Web.Controllers
{
    public class CartController : Controller
    {
        ICartService _cartService;
        IAccountService _accountService;
        IProductService _productService;
        public CartController(ICartService cartService, IAccountService accountService, IProductService productService)
        {
            _cartService = cartService;
            _accountService = accountService;
            _productService = productService;
        }
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var listCart = _cartService.GetAllByUsername(username);
            ViewBag.Message = TempData["Message"];
            ViewBag.ProductId = TempData["ProductId"];
            ViewBag.Var = TempData["Var"];
            return View(listCart);
        }
        public IActionResult AddToCart(Cart c, string returnUrl)
        {
            if (string.IsNullOrEmpty(c.Username))
            {
                return RedirectToAction("Index", "Login");
            }

            string result = _cartService.AddOrUpdateToCart(c, c.Quantity);

            string separator = returnUrl.Contains('?') ? "&" : "?";
            string redirectUrl;

            if (result == "OK")
            {
                redirectUrl = $"{returnUrl}{separator}msg=Added successfully&pid={c.ProductId}";
            }
            else
            {
                redirectUrl = $"{returnUrl}{separator}error={Uri.EscapeDataString(result)}&pid={c.ProductId}";
            }

            return Redirect(redirectUrl);
        }

        public IActionResult UpdateQuantity(int productId, int delta, int var)
        {
            var username = HttpContext.Session.GetString("Username");
            var cartItem = new Cart { Username = username, ProductId = productId, VariantId = var };

            var result = _cartService.AddOrUpdateToCart(cartItem, delta);
            TempData["Message"] = result != "OK" ? result : "";
            TempData["ProductId"] = productId;
            TempData["Var"] = var;
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult DeleteItemInCart(int productId)
        {
            var username = HttpContext.Session.GetString("Username");
            var cartItem = new Cart { Username = username, ProductId = productId };
            _cartService.Delete(cartItem);
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult GoCheckout(int[] selectedProducts)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (selectedProducts == null || selectedProducts.Length == 0)
            {
                TempData["Message"] = "Please select at least one product to proceed to checkout.";
                return RedirectToAction("Index", "Cart");
            }
            var account = _accountService.GetAccountByUsername(username);
            List<Cart> listCheckout = new List<Cart>();
            listCheckout = _cartService.GetProductToCheckOut(username, selectedProducts);

            foreach (var p in listCheckout)
            {
                var product = _productService.GetProductById(p.Product.ProductId);
                int? inStock = product.UnitsInStock;

                if (inStock < p.Quantity)
                {
                    int? reduceBy = p.Quantity - inStock;

                    _cartService.AddOrUpdateToCart(p, (int)-reduceBy);

                    TempData["Message"] = $"Not enough stock for product '{product.ProductName}', only {inStock} left.";
                    TempData["ProductId"] = p.Product.ProductId;

                    return RedirectToAction("Index", "Cart");
                }
            }

            ViewBag.Account = account;
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(listCheckout, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            return View("/Views/Checkout/Index.cshtml", listCheckout);
        }
    }
}
