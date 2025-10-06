using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Services;

namespace Shopping_Web.Controllers
{
    public class DesignController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public DesignController(IEmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SaveImage(IFormFile canvasFile, List<IFormFile> userFiles, string fullname, string phone, string email, int quantity, string note)
        {
            if (canvasFile == null && userFiles == null)
                return BadRequest("No files uploaded.");

            string subject = "Báo giá thiết kế - The InkSideCrew";
            string body = $@"
                <h3>Cám ơn bạn đã liên hệ tới The InkSide.</h3>
                <p>Báo giá của bạn đã được gửi tới thiết kế. 
                Dưới đây là thông tin bạn đã gửi:</p>
                <ul>
                    <li><b>Họ tên:</b> {fullname}</li>
                    <li><b>Số điện thoại:</b> {phone}</li>
                    <li><b>Email:</b> {email}</li>
                    <li><b>Số lượng:</b> {quantity}</li>
                    <li><b>Nội dung:</b> {note}</li>
                </ul>
                <p>Chúng tôi sẽ liên hệ lại bạn sớm nhất.</p>
                <p>Trân trọng cảm ơn,<br/>The InkSideCrew</p>
            ";

            var attachments = new List<Attachment>();

            if (canvasFile != null && canvasFile.Length > 0)
            {
                var ms = new MemoryStream();
                await canvasFile.CopyToAsync(ms);
                ms.Position = 0;
                attachments.Add(new Attachment(ms, canvasFile.FileName, canvasFile.ContentType));
            }

            if (userFiles != null && userFiles.Count > 0)
            {
                foreach (var file in userFiles)
                {
                    var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    ms.Position = 0;
                    attachments.Add(new Attachment(ms, file.FileName, file.ContentType));
                }
            }



            await _emailService.SendEmailAsync(email, subject, body,attachments);
            var adminEmail = _configuration["EmailSettings:AdminEmail"];
            await _emailService.SendEmailAsync(adminEmail, "New Contact Form Submission", body,attachments);

            foreach (var att in attachments)
                att.ContentStream.Dispose();


            return Json(new
            {
                success = true,
                fullname,
                phone,
                email,
                quantity,
                note
            });
        }
    }
}
