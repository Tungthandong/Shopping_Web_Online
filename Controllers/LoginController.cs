using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shopping_Web.Models;
using Shopping_Web.Services;
using Shopping_Web.ViewModels;

namespace Shopping_Web.Controllers
{
    public class LoginController : Controller
    {
        IAccountService _accountService;
        public LoginController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }
        [HttpPost]
        public IActionResult Login()
        {
            string username = Request.Form["Username"];
            string password = Request.Form["Password"];
            bool remember = Request.Form["Remember"] == "on";
            TempData["ShowWelcomeDialog"] = false;

            var account = _accountService.GetAccountByUsername(username);
            if (account != null)
            {
                var hasher = new PasswordHasher<Account>();
                var result = hasher.VerifyHashedPassword(account, account.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("Username", account.Username);
                    HttpContext.Session.SetString("Role", account.Role);

                    if (remember)
                    {
                        var options = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(7),
                            IsEssential = true,
                            HttpOnly = true
                        };
                        Response.Cookies.Append("Username", account.Username, options);
                        Response.Cookies.Append("Remember", remember.ToString(), options);
                    }
                    else
                    {
                        Response.Cookies.Delete("Username");
                        Response.Cookies.Delete("Remember");
                    }

                    if (TempData["NewRegister"] != null && (bool)TempData["NewRegister"] == true)
                    {
                        TempData["ShowWelcomeDialog"] = true;
                        TempData["NewRegister"] = false;
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid username or password!!!";
            return View("~/Views/Login/Index.cshtml");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public IActionResult Signup(Signup signup)
        {
            string message = string.Empty;

            if (_accountService.GetAccountByUsername(signup.Username) != null)
            {
                message = "Account is existed!!!";
                TempData["Message"] = message;
                return RedirectToAction("Index", "Login");
            }

            if (!signup.ConfirmPassword.Equals(signup.Password))
            {
                message = "Password and confirmation do not match!!!";
                TempData["Message"] = message;
                return RedirectToAction("Index", "Login");
            }
            var hasher = new PasswordHasher<Account>();
            var account = new Account
            {
                Username = signup.Username,
                Email = signup.Email,
                Phonenumber = signup.Phonenumber,
                AccountStatus = "active",
                Role = "customer"
            };
            account.Password = hasher.HashPassword(account, signup.Password);

            _accountService.AddAccount(account);

            message = "Signup Successfully. Please login and add more information in Account";
            TempData["Message"] = message;
            TempData["NewRegister"] = true;

            return RedirectToAction("Index", "Login");
        }
    }
}
