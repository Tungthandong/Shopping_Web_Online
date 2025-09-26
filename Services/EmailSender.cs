using System.Net;
using System.Net.Mail;

namespace Shopping_Web.Services
{
    public class EmailSender:IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var from = _config["EmailSettings:From"];
            var password = _config["EmailSettings:AppPassword"];
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"]);

            using (var client = new SmtpClient(smtpServer, port))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(from, password);

                var mailMessage = new MailMessage(from, toEmail, subject, body)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
