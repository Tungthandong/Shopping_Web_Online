using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Models;
using Shopping_Web.Services;
using Shopping_Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shopping_Web.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public EmailController(IEmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _configuration = configuration;
        }
        public async Task<IActionResult> Submit(ContactForm model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Content("Invalid data: " + string.Join(", ", errors));
            }


            string filePath = null;
            byte[] fileBytes = null;
            string fileName = null;

            // Nếu có file upload
            if (model.File != null && model.File.Length > 0)
            {
                fileName = Path.GetFileName(model.File.FileName);
                using var ms = new MemoryStream();
                await model.File.CopyToAsync(ms);
                fileBytes = ms.ToArray(); // dùng để attach vào email
            }

            string subject = "Báo giá thiết kế - The InkSideCrew";
            string body = $@"
                <h3>Cám ơn bạn đã liên hệ tới The InkSide.</h3>
                <p>Báo giá của bạn đã được gửi tới thiết kế. 
                Dưới đây là thông tin bạn đã gửi:</p>
                <ul>
                    <li><b>Họ tên:</b> {model.FullName}</li>
                    <li><b>Số điện thoại:</b> {model.Phone}</li>
                    <li><b>Email:</b> {model.Email}</li>
                    <li><b>Số lượng:</b> {model.Quantity}</li>
                    <li><b>Nội dung:</b> {model.Note}</li>
                </ul>
                <p>Chúng tôi sẽ liên hệ lại bạn sớm nhất.</p>
                <p>Trân trọng cảm ơn,<br/>The InkSideCrew</p>
            ";

            await _emailService.SendEmailAsync(model.Email, subject, body);
            var adminEmail = _configuration["EmailSettings:AdminEmail"];
            await _emailService.SendEmailAsync(adminEmail, "New Contact Form Submission", body);
            TempData["Message"] = "Yêu cầu của bạn đã được gửi thành công!";
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
