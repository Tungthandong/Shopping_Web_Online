namespace Shopping_Web.Services
{
    public interface IEmailService
    {
<<<<<<< HEAD
        Task SendEmailAsync(string email, string subject, string message);
=======
        Task SendEmailAsync(string toEmail, string subject, string body);
>>>>>>> ed068d502c9d5e2f55561b23e062ee5542cd2786
    }
}
