using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Models;
using Shopping_Web.Services;
using Shopping_Web.ViewModels;

namespace Shopping_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
                return RedirectToAction("Index", "Login");

            var account = _accountService.GetAccountByUsername(username);
            return View(account);
        }

        [HttpPost]
        public IActionResult UpdateProfile(UpdateProfile updateProfile)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Thông tin không hợp lệ. Vui lòng kiểm tra lại.";
                return RedirectToAction("Index", "Account");
            }

            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                TempData["Message"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Index", "Login");
            }

            var account = _accountService.GetAccountByUsername(username);
            if (account == null)
            {
                TempData["Message"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("Index", "Account");
            }

            account.Fullname    = updateProfile.Fullname;
            account.Phonenumber = updateProfile.Phonenumber;
            account.Email       = updateProfile.Email;
            account.Birthdate   = updateProfile.Birthdate;
            account.Gender      = updateProfile.Gender;
            _accountService.UpdateProfile(account);

            TempData["Message"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePassword changePassword)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Thông tin không hợp lệ. Vui lòng kiểm tra lại.";
                return RedirectToAction("Index", "Account");
            }

            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                TempData["Message"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Index", "Login");
            }

            var account = _accountService.GetAccountByUsername(username);
            if (account == null)
            {
                TempData["Message"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("Index", "Account");
            }

            var hasher = new PasswordHasher<Account>();

            var verifyResult = hasher.VerifyHashedPassword(account, account.Password, changePassword.CurrentPassword);
            if (verifyResult != PasswordVerificationResult.Success)
            {
                TempData["Message"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToAction("Index", "Account");
            }

            if (changePassword.NewPassword == changePassword.CurrentPassword)
            {
                TempData["Message"] = "Mật khẩu mới phải khác mật khẩu hiện tại.";
                return RedirectToAction("Index", "Account");
            }

            account.Password = hasher.HashPassword(account, changePassword.NewPassword);
            _accountService.UpdateProfile(account);

            TempData["Message"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index", "Account");
        }
    }
}
