using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping_Web.Models;
using Shopping_Web.Models.Vnpay;
using Shopping_Web.Services;
using Shopping_Web.Services.Vnpay;

namespace Shopping_Web.Controllers
{
    public class CheckoutController : Controller
    {
        private IOrderService _orderService;
        private ICartService _cartService;
        private IProductService _productService;
        private IVnPayService _vnPayService;

        public CheckoutController(IOrderService orderService, ICartService cartService, IProductService productService, IVnPayService vnPayService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _productService = productService;
            _vnPayService = vnPayService;
        }
        public IActionResult Index()
        {
            ViewBag.Msg = TempData["Msg"];
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public IActionResult Checkout(string PhoneNumber, string ShippingAddress, string PaymentMethod, string ShippingMethod, int TotalAmount)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username==null)
            {
                return RedirectToAction("Index", "Login");
            }
            Order order = new Order()
            {
                Username = username,
                ShippingAddress = ShippingAddress,
                PaymentMethod = PaymentMethod,
                ShipCost = (ShippingMethod == "express" ? 50000 : 30000),
                OrderDate = DateTime.Now,
                OrderStatus = (PaymentMethod == "VnPay" ? "Cancelled" : "Pending"),
                PhoneNumber = PhoneNumber,
                TotalAmount = TotalAmount
            };
            _orderService.AddOrder(order);
            var listCart = HttpContext.Session.GetString("Cart");
            List<Cart> cart = JsonConvert.DeserializeObject<List<Cart>>(listCart);
            _orderService.AddOrderDetail(order.OrderId, cart);
            foreach (var c in cart)
            {
                _productService.UpdateQuantity(_productService.GetProductById(c.Product.ProductId), _productService.GetProductVariantById(c.VariantId), c.Quantity, "-");
                _cartService.Delete(c);
            }
            HttpContext.Session.Remove("Cart");

            if (PaymentMethod == "VnPay")
            {
                PaymentInformationModel model = new PaymentInformationModel()
                {
                    Name = username,
                    Amount = TotalAmount,
                    OrderDescription = "Checkout by VNPAY,time:"+DateTime.Now,
                    OrderType = "other"
                };

                return Redirect(_vnPayService.CreatePaymentUrl(model, HttpContext));
            }

            TempData["Msg"] = "Checkout successfully";
            return RedirectToAction("Index", "Checkout");
        }
        public IActionResult OrderHistory(int page = 1)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int pagesize = 4;
            var listOrder = _orderService.GetOrdersByUser(username);
            int totalpage = (int)Math.Ceiling((decimal)listOrder.Count() / pagesize);
            var pagedOrder = listOrder
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();
            ViewBag.Totalpage = totalpage;
            ViewBag.Pageindex = page;
            return View(pagedOrder);
        }

        public IActionResult CancelOrder(int oid)
        {
            var username = HttpContext.Session.GetString("Username");
            var listOrder = _orderService.GetOrdersByUser(username);
            Order order = listOrder.FirstOrDefault(o => o.OrderId == oid);
            if (order == null)
            {
                TempData["Msg"] = "Order not exist";
                return RedirectToAction("OrderHistory", "Checkout");
            }
            var detail = _orderService.getDetailsByOrderId(oid);
            foreach (var d in detail)
            {
                _productService.UpdateQuantity(_productService.GetProductById(d.ProductId), _productService.GetProductVariantById((int)d.VariantId), d.Quantity, "+");  
            }
            _orderService.CancelOrder(oid);
            return RedirectToAction("OrderHistory", "Checkout");
        }
        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.VnPayResponseCode == "00")
            {
                var username = HttpContext.Session.GetString("Username");
                Order order = _orderService.GetOrdersByUser(username)[0];
                _orderService.UpdateOrderStatus(order.OrderId, "Pending");
                TempData["Msg"] = "Checkout successfully";
            }
            else
            {
                TempData["Msg"] = "Checkout failed";
            }
            return RedirectToAction("Index", "Checkout");
        }
    }
}
