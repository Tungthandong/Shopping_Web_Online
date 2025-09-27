    public class EmailService:IEmailService
    public class EmailService : IEmailService
namespace Shopping_Web.Services
        private readonly IConfiguration _config;
        private readonly IConfiguration _configuration;
    public class EmailService : IEmailService
        public EmailService(IConfiguration config)
        public EmailService(IConfiguration configuration)
        private readonly IConfiguration _configuration;
            _config = config;
            _configuration = configuration;
        public EmailService(IConfiguration configuration)
        {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        public async Task SendEmailAsync(string to, string subject, string body)
        }
            var from = _config["EmailSettings:From"];
            var password = _config["EmailSettings:AppPassword"];
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            using (var client = new SmtpClient(smtpServer, port))
            {
            using (var client = new System.Net.Mail.SmtpClient(smtpServer, smtpPort))
            {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];

            using (var client = new System.Net.Mail.SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                client.EnableSsl = true;

                var mail = new System.Net.Mail.MailMessage(senderEmail, to, subject, body);
                mail.IsBodyHtml = true;
                await client.SendMailAsync(mailMessage);
                var mail = new System.Net.Mail.MailMessage(senderEmail, to, subject, body);
                mail.IsBodyHtml = true;
                await client.SendMailAsync(mail);
            }
        }
    }
}