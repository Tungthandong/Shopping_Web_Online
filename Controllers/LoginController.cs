using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shopping_Web.Models;
using Shopping_Web.Services;
using Shopping_Web.ViewModels;
using System.Security.Claims;

namespace Shopping_Web.Controllers
{
    public class LoginController : Controller
    {
        IAccountService _accountService;
        Services.IEmailService _emailSender;
        public LoginController(IAccountService accountService, Services.IEmailService emailSender)
        {
            _accountService = accountService;
            _emailSender = emailSender;
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
        public async Task<IActionResult> Signup(Signup signup)
        {
            string message = string.Empty;

            if (_accountService.GetAccountByUsername(signup.Username) != null)
            {
                message = "Account is existed!!!";
                TempData["Message"] = message;
                return RedirectToAction("Index", "Login");
            }

            if (_accountService.GetAccountByEmail(signup.Email, "active") != null)
            {
                message = "Email is registered!!!";
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
                AccountStatus = "inactive",
                Role = "customer",
                CreateDate = DateTime.Now
            };
            account.Password = hasher.HashPassword(account, signup.Password);

            _accountService.AddAccount(account);

            await SendMailVerified(account.Email);

            message = "Signup Successfully. Please check your email to verify your account.";
            TempData["Message"] = message;
            TempData["NewRegister"] = true;

            return RedirectToAction("Index", "Login");
        }


        public async Task LoginGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", "Login");
            }
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string emailName = email.Split('@')[0];
            var user = _accountService.GetAccountByEmail(email, "active");
            if (user == null)
            {
                var hasher = new PasswordHasher<Account>();
                var account = new Account
                {
                    Username = emailName,
                    Email = email,
                    AccountStatus = "active",
                    Role = "customer",
                    CreateDate = DateTime.UtcNow
                };
                account.Password = hasher.HashPassword(account, "123456789");

                _accountService.AddAccount(account);

                var res = hasher.VerifyHashedPassword(account, account.Password, "123456789");

                if (res == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("Username", account.Username);
                    HttpContext.Session.SetString("Role", account.Role);

                    TempData["ShowWelcomeDialog"] = true;
                    TempData["NewRegister"] = false;

                    return RedirectToAction("Index", "Home");
                }
            }
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPass()
        {
            ViewBag.msg = TempData["success"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMailForgotPass(string email)
        {
            var checkMail = _accountService.GetAccountByEmail(email, "active");

            if (checkMail == null)
            {
                TempData["error"] = "Email not found";
                return RedirectToAction("ForgotPass", "Login");
            }
            else
            {
                var receiver = checkMail.Email;
                var subject = "Change password for user " + checkMail.Email;
                var resetLink = Url.Action("NewPass", "Login",
                                new { email = checkMail.Email }, Request.Scheme);

                var message = $"Click on the link to change password: <a href='{resetLink}'>Reset Password</a>";

                await _emailSender.SendEmailAsync(receiver, subject, message);
            }


            TempData["success"] = "An email has been sent to your registered email address with password reset instructions.";
            return RedirectToAction("ForgotPass", "Login");
        }
        public async Task SendMailVerified(string email)
        {
            var checkMail = _accountService.GetAccountByEmail(email, "inactive");

            if (checkMail == null)
            {
                return;
            }
            else
            {
                var receiver = checkMail.Email;
                var subject = "Verify email for user " + checkMail.Email;
                var resetLink = Url.Action("VerifyEmail", "Login",
                                new { email = checkMail.Email }, Request.Scheme);

                var message = $"Click on the link to verify: <a href='{resetLink}'>Verify Email</a>";

                await _emailSender.SendEmailAsync(receiver, subject, message);
            }
        }
        public async Task<IActionResult> NewPass(string email)
        {
            var checkuser = _accountService.GetAccountByEmail(email, "active");

            if (checkuser != null)
            {
                ViewBag.Email = checkuser.Email;
            }
            else
            {
                TempData["error"] = "Email not found";
                return RedirectToAction("ForgotPass", "Login");
            }
            return View();
        }
        public async Task<IActionResult> UpdateNewPassword(string email, string newpass)
        {
            var checkuser = _accountService.GetAccountByEmail(email, "active");

            if (checkuser != null)
            {
                var passwordHasher = new PasswordHasher<Account>();
                var passwordHash = passwordHasher.HashPassword(checkuser, newpass);

                checkuser.Password = passwordHash;

                _accountService.UpdateProfile(checkuser);
                TempData["success"] = "Password updated successfully.";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                TempData["error"] = "Email not found";
                return RedirectToAction("ForgotPass", "Login");
            }
            return View();
        }
        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            var account = _accountService.GetAccountByEmail(email, "inactive");
            if (account == null)
            {
                TempData["Message"] = "Account not found!";
                return RedirectToAction("Index", "Login");
            }

            account.AccountStatus = "active";
            _accountService.UpdateProfile(account);

            TempData["Message"] = "Email verified successfully!";
            return RedirectToAction("Index", "Login");
        }
    }
}
