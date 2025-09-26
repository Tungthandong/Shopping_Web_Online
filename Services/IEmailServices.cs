namespace Shopping_Web.Services
{
    public interface IEmailServices
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
