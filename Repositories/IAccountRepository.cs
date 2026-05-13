using Shopping_Web.Models;

namespace Shopping_Web.Repositories
{
    public interface IAccountRepository
    {
        Account? Login(string username, string password);
        Account? GetAccountByUsername(string username);
        Account? GetAccountByEmail(string email, string status);
        void UpdateProfile(Account a);
        void AddAccount(Account a);
        List<Account> GetAccounts();
        string UpdateAccountStatus(string username, string status);
    }
}
