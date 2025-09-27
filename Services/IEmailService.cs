namespace Shopping_Web.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}