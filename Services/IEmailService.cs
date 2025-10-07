using System.Net.Mail;

namespace Shopping_Web.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);

        Task SendEmailAsync(string toEmail, string subject, string body,List<Attachment> attachments);
    }
}