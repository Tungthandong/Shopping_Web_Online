using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Models;
using Shopping_Web.Services;
using Shopping_Web.ViewModels;
using System.Reflection;

namespace Shopping_Web.Controllers
{
    public class AccountController : Controller
    {
        IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            var username = HttpContext.Session.GetString("Username");
            if (username==null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (username!=null)
            {
                var account = _accountService.GetAccountByUsername(username);
                return View(account);
            }
            return View();
        }
        [HttpPost]
        public IActionResult UpdateProfile(UpdateProfile updateProfile) {
            var username = HttpContext.Session.GetString("Username");
            var message = "Update Failed!!!";
            if (username!=null)
            {
                var account = _accountService.GetAccountByUsername(username);
                if (account != null)
                {
                    account.Fullname = updateProfile.Fullname;
                    account.Phonenumber = updateProfile.Phonenumber;
                    account.Email = updateProfile.Email;
                    account.Birthdate = updateProfile.Birthdate;
                    account.Gender = updateProfile.Gender;
                    _accountService.UpdateProfile(account);
                    message = "Update Successfully";
                }
            }
            TempData["Message"] = message;
            return RedirectToAction("Index", "Account");
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasword changePasword)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                TempData["Message"] = "You must login first!!!";
                return RedirectToAction("Login");
            }

            var account = _accountService.GetAccountByUsername(username);
            if (account == null)
            {
                TempData["Message"] = "Account not found!!!";
                return RedirectToAction("Login");
            }

            var hasher = new PasswordHasher<Account>();
            
            var verifyResult = hasher.VerifyHashedPassword(account, account.Password, changePasword.CurrentPassword);
            if (verifyResult != PasswordVerificationResult.Success)
            {
                TempData["Message"] = "Wrong current password!!!";
                return RedirectToAction("Index", "Account");
            }

            if (changePasword.NewPassword != changePasword.ConfirmPassword)
            {
                TempData["Message"] = "New password and confirmation do not match!!!";
                return RedirectToAction("Index", "Account");
            }

            if (changePasword.NewPassword == changePasword.CurrentPassword)
            {
                TempData["Message"] = "New password must be different from current password!!!";
                return RedirectToAction("Index", "Account");
            }

            account.Password = hasher.HashPassword(account, changePasword.NewPassword);
            _accountService.UpdateProfile(account);

            TempData["Message"] = "Change Successfully";
            return RedirectToAction("Index", "Account");
        }
    }
}
